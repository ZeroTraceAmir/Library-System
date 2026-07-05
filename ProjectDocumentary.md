# Library Management System — Project Documentary

---

## 1. Executive Summary

The **Library Management System** is a Windows Forms desktop application built for small-to-medium Persian-language libraries transitioning from manual or spreadsheet-based operations to a digital workflow. It solves the core operational triad of library management:

- **Inventory tracking** — which books exist, how many copies are available, and where they are.
- **Patron management** — who has borrowed what, when it is due, and what they owe.
- **Notifications & enforcement** — automatic alerts for overdue items and return reminders.

The system serves three distinct user roles:

| Role | Scope |
|------|-------|
| **Admin** | Full system control: manage employees (add/delete), view all customers, access all administrative panels |
| **Staff** | Library operations: add/edit/delete books, view customer lists, manage inventory |
| **Customer** | Self-service: browse books, borrow & return, reserve, pay debts, view notifications |

The target audience is any library with a Persian-speaking staff and customer base that needs a lightweight, zero-infrastructure digital solution — no database server required, no internet dependency, no licensing costs. The entire system runs on a single Windows machine with JSON files as the persistence layer.

---

## 2. System Architecture & Tech Stack

### 2.1 Technology Choices

| Layer | Technology | Rationale |
|-------|-----------|-----------|
| Language | **C# 13** (`.NET 10.0`) | Modern, type-safe, excellent Windows Forms integration; nullable reference types reduce null-reference bugs at compile time |
| UI Framework | **Windows Forms** | Rapid development for desktop data-entry applications; native RTL support essential for Persian UI; no web server required |
| Persistence | **JSON files** (`System.Text.Json`) | Zero setup, human-readable, easy to inspect/debug; `JsonDerivedType` attributes solve polymorphic serialization that would require a full ORM with a relational DB |
| Build System | **MSBuild / dotnet CLI** | Standard .NET tooling; `dotnet csharpier` for automated formatting |
| IDE Support | **Visual Studio Code / C# Dev Kit** | Lightweight, cross-platform editing experience even though the target is Windows |

### 2.2 Architecture: Three-Layer + JSON Store

The system follows a strict three-layer architecture with a JSON file store at the bottom:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer (Forms)                │
│  Form1 | LoginForm | RegisterForm | Home | AdminPanel       │
│  StaffPanel | Profile | SeeBooks | SeeAllBooks              │
│  SeeAllCustomers | SeeAllEmployees | MyLoans | MyReservations│
│  PayDebt | AddBook | AddEmployee | Notifications            │
├─────────────────────────────────────────────────────────────┤
│                    Business Logic Layer (Services)           │
│  BookService : BaseService<Book>                            │
│  CustomerService : BaseService<Customer>                    │
│  UserService : BaseService<User>                            │
│  LoanService | ReservationService | DebtService             │
│  NotificationService                                        │
├─────────────────────────────────────────────────────────────┤
│                    Data Access Layer (Repositories)          │
│  JsonRepository<T> : IRepository<T>  where T : IEntity     │
│  JsonBookRepository | JsonCustomerRepository | ...          │
│  JsonDataStore (generic Load<T> / Save<T>)                  │
├─────────────────────────────────────────────────────────────┤
│                    Persistence (JSON Files)                   │
│  Data/books.json | customers.json | users.json              │
│  Data/loans.json | debts.json | reservations.json           │
│  Data/notifications.json                                    │
└─────────────────────────────────────────────────────────────┘
```

**Key architectural decisions:**

- **Manual dependency injection** — no DI container (Microsoft.Extensions.DependencyInjection or similar). Each form constructs its own `JsonDataStore`, repository, and service instances inline. This keeps the dependency graph fully visible and avoids the complexity of service registration for a project of this size. The trade-off is tighter coupling between forms and concrete types, but the simplicity gain outweighs it for a 17-form application.

- **Interface-based repository contracts** — every repository implements `IRepository<T>` (constrained to `IEntity`), which allows `JsonRepository<T>` to use `T.Id` without reflection. The sub-interfaces (`IBookRepository`, `ICustomerRepository`, etc.) exist purely for type-safety in constructor injection — they add no members beyond `IRepository<T>`.

- **Abstract base classes over pure interfaces** — the model hierarchy (`BaseEntity → Person → Account → Customer/User`) uses abstract classes to share implementation (the `Id` property, `Number`, `Password`, `IsLogedin`) while reserving interfaces for polymorphic substitution. This is a pragmatic blend of implementation inheritance and interface contract.

### 2.3 Data Flow

```
User Action (button click in Form)
    → Form calls method on Service
        → Service calls Validate() (if applicable)
            → Service calls Repository method
                → Repository calls JsonDataStore.Load<T>() / Save<T>()
                    → File I/O on Data/*.json
        → Service fires event (e.g., BookBorrowed?.Invoke)
            → Event handler in Form creates Notification
```

---

## 3. Core Features & Functionality

### 3.1 Authentication & Role Routing

The login system handles three distinct user types through a single entry point:

```csharp
// Program.cs — entry point routing
if (UserLoggedIn != null)
{
    Form form = UserLoggedIn.Role == Enums.UserStatus.admin
        ? new AdminPanel(UserLoggedIn)
        : new StaffPanel(UserLoggedIn);
    Application.Run(form);
}
else if (CustomerLoggedIn != null)
{
    Application.Run(new Home(CustomerLoggedIn));
}
else
{
    Application.Run(new Form1());  // login/register screen
}
```

`Program.cs` checks for existing login sessions (`IsLogedin` flag persisted in JSON) on startup. If a user is already logged in, they skip directly to their panel — a convenience for single-user library kiosks.

`Form1` (the login/register gateway) uses `LoginForm` which accepts both `CustomerService` and `UserService`, enabling a single form to authenticate all three roles. The `DialogResult` pattern passes the authenticated role back to `Form1` which then routes to the correct panel.

### 3.2 Book Borrowing & Returning

**Borrowing** (`LoanService.BorrowBook`):

1. Validates `book.CopiesAvailable > 0`
2. Decrements `CopiesAvailable`
3. Creates a `Loan` record with `DueDate = DateTime.Now.AddMinutes(2)` (intentionally short for testing)
4. Fires `BookBorrowed` event

**Returning** (`LoanService.ReturnBook`):

1. Loads the loan by ID; throws if not found or already returned
2. Sets `loan.ReturnDate = DateTime.Now`
3. Increments `CopiesAvailable`
4. If overdue (returned after `DueDate`), calculates a late fee:
   - `DailyLateFee = 5000` (Iranian Rial, hardcoded)
   - `amount = daysOverdue * DailyLateFee`
   - Creates a `Debt` record and updates `customer.Debt`
5. Fires `BookReturned` event

The returned data is idempotent — calling `ReturnBook` twice on the same loan throws `"این کتاب قبلاً بازگردانده شده است"` (This book has already been returned).

### 3.3 Reservation System

Customers can reserve books when all copies are currently borrowed. The `ReservationService.ReserveBook` method:

1. Creates a `Reservation` with `IsActive = true`
2. Fires `BookReserved` event, which triggers `NotificationService.CreateReservationConfirmedNotification`

Reservations can be cancelled (`CancelReservation`), which sets `IsActive = false` and fires `ReservationCancelled` notification.

### 3.4 Notification System

The notification system demonstrates the project's most sophisticated polymorphic design:

```
Notification (abstract, BaseEntity)
  └── BookNotification (abstract)
       ├── BookBorrowedNotification
       ├── ReservationConfirmedNotification
       ├── ReservationCancelledNotification
       └── LoanNotification (abstract)
            ├── OverdueNotification
            └── ReturnReminderNotification
```

Each concrete type overrides `GetMessage()` with its own Persian message. The `NotificationService` provides:

- **Create methods** — one per notification type
- **`CheckOverdueAndReminders`** — iterates all active loans and inspects existing notifications using `is` pattern matching to avoid duplicates:

```csharp
bool overdueExists = allNotifications.Any(n =>
    n is OverdueNotification
    && n.CustomerId == loan.CustomerId
    && n is BookNotification bn
    && bn.BookId == loan.BookId
);
```

The `Notifications.cs` form displays notifications by calling `notification.GetMessage()` on the abstract base — runtime dispatch ensures the correct message appears.

Notifications are persisted in `Data/notifications.json`. The `[JsonDerivedType]` attributes on the `Notification` class tell `System.Text.Json` how to serialize/deserialize the concrete types through the abstract base.

### 3.5 Debt Tracking & Payment

Debts are created automatically when books are returned late. Each debt records the customer, amount, reason (Persian description including the book title and days overdue), and payment status. Customers can view and pay their debts through `PayDebt.cs`, which marks `IsPaid = true`.

### 3.6 User & Customer Management

**Admin features:**
- View all employees with role-based filtering (`UserFilter`: All, Admins, Staff)
- Add new employees with role selection (admin/staff)
- Delete employee accounts

**Staff features:**
- View all books with search (title, author) and genre filtering
- Add new books to inventory
- View all customers with status filtering (`CustomerFilter`: All, HasBorrowed, HasReserved, HasDebt)

**Customer self-service:**
- View and edit profile
- Delete own account
- View borrowing history and active loans
- Return books
- Manage reservations
- Pay debts

---

## 4. Technical Implementation & Code Walkthrough

### 4.1 Project Structure

```
LibrarySystem/
├── Program.cs                    # Application entry point with routing
├── Form1.cs                      # Login/Register gateway
├── *.cs                          # 15 additional Forms
├── Interfaces/                   # 15 interface files
│   ├── IEntity.cs                # Base entity contract (int Id)
│   ├── IRepository.cs            # Generic CRUD contract (where T : IEntity)
│   ├── IBook.cs, ICustomer.cs,   # Domain model interfaces
│   ├── IBookRepository.cs, ...   # Repository sub-interfaces
├── Models/                       # 14 model classes
│   ├── BaseEntity.cs             # Abstract base (Id)
│   ├── Person.cs                 # Abstract (Name)
│   ├── Account.cs                # Abstract (Number, Password, IsLogedin, virtual GetRoleLabel)
│   ├── Customer.cs               # Concrete (overrides GetRoleLabel)
│   ├── User.cs                   # Concrete (Role + overrides GetRoleLabel)
│   ├── Book.cs, Loan.cs, ...     # Concrete entities
│   └── Notification.cs + 5 derived notification types
├── Enums/                        # 4 enum files
│   ├── UserStatus.cs             # admin, staff
│   ├── CustomerFilter.cs         # All, HasBorrowed, HasReserved, HasDebt
│   ├── UserFilter.cs             # All, Admins, Staff
│   └── NotificationType.cs       # Overdue, ReturnReminder, etc.
├── Repositories/                 # 8 repository files
│   ├── JsonDataStore.cs          # Generic JSON read/write
│   ├── JsonRepository.cs         # Generic CRUD implementation
│   └── JsonBookRepository.cs     # 7 concrete repositories
├── Services/                     # 7 service files
│   ├── BaseService.cs            # Abstract generic service
│   ├── BookService.cs, UserService.cs, CustomerService.cs
│   ├── LoanService.cs, ReservationService.cs
│   ├── DebtService.cs, NotificationService.cs
└── Data/                         # JSON persistence files
    ├── books.json, customers.json, users.json
    ├── loans.json, debts.json, reservations.json
    └── notifications.json
```

### 4.2 Key Design Patterns

#### Generic Repository Pattern

`IRepository<T>` constrains `T` to `IEntity`, which guarantees `T.Id` exists. `JsonRepository<T>` implements all five CRUD operations using `JsonDataStore` for serialization:

```csharp
public class JsonRepository<T> : IRepository<T> where T : IEntity
{
    public T? GetById(int id)
    {
        List<T>? items = GetAll();
        T item = items.FirstOrDefault(x => x.Id == id);
        return item;
    }
}
```

The concrete repositories (e.g., `JsonBookRepository`) simply specify the file path and inherit everything:

```csharp
public class JsonBookRepository : JsonRepository<Book>, IBookRepository
{
    public JsonBookRepository(JsonDataStore dataStore)
        : base(dataStore, "books.json") { }
}
```

#### Template Method Pattern (Validation)

`BaseService<T>` defines an abstract `Validate(T entity)` method. Each service provides its own implementation:

| Service | Validation Rules |
|---------|-----------------|
| `BookService.Validate` | Not null, title required, author required, year > 0, copies >= 0 |
| `CustomerService.Validate` | Not null, name required, phone required |
| `UserService.Validate` | Not null, name required, phone required, password match |

#### Strategy Pattern (Filtering)

`CustomerFilter` and `UserFilter` enums plus `switch` expressions in `GetFilteredCustomers` and `GetFilteredUsers` implement a lightweight Strategy pattern:

```csharp
return filter switch
{
    CustomerFilter.HasBorrowed => customers.Where(c => c.HasBorrowedBook).ToList(),
    CustomerFilter.HasReserved => customers.Where(c => c.HasReservedBook).ToList(),
    CustomerFilter.HasDebt => customers.Where(c => c.Debt > 0).ToList(),
    _ => customers,
};
```

#### Event/Delegate Pattern (Cross-Cutting Notifications)

Custom delegate types decouple the borrowing/reservation operations from notification creation:

- **`BookEventHandler(Book book, Loan? loan, int customerId)`** — used by `LoanService.BookBorrowed`, `LoanService.BookReturned`, `ReservationService.BookReserved`
- **`CustomerEventHandler(Customer customer)`** — used by `CustomerService.CustomerRegistered`

Forms subscribe to these events with lambdas:

```csharp
// SeeBooks.cs
loanService.BookBorrowed += (book, _, customerId) =>
    notificationService.CreateBookBorrowedNotification(customerId, book.Id);

// RegistrationForm.cs
customerService.CustomerRegistered += c =>
    ShowMessage($"خوش آمدید {c.Name}! ثبت نام با موفقیت انجام شد", ...);
```

This is the Observer pattern — services emit events without knowing who (or how many) are listening.

### 4.3 Polymorphism Inventory

The project uses **15 distinct polymorphic patterns**:

| # | Pattern | Mechanism |
|---|---------|-----------|
| 1 | Interface inheritance (`IBook : IEntity`, etc.) | Interface extension |
| 2 | Repository interfaces (`IRepository<T> + 7 sub-interfaces`) | Generic constraint |
| 3 | Abstract inheritance (`BaseEntity → Person → Account`) | Multi-level abstract chain |
| 4 | Notification hierarchy (3 levels deep) | Abstract chain with `[JsonDerivedType]` |
| 5 | Virtual method dispatch (`Account.GetRoleLabel()`) | Runtime virtual dispatch |
| 6 | Abstract method (`Notification.GetMessage()`) | Runtime dispatch on base variable |
| 7 | Abstract method (`BaseService<T>.Validate(T)`) | Template Method pattern |
| 8 | Generic constraint (`where T : IEntity`) | Compile-time polymorphism |
| 9 | Generic methods (`JsonDataStore.Load<T>()`) | Unconstrained generics |
| 10 | `is` pattern matching (`n is OverdueNotification`) | Runtime type inspection |
| 11 | Base-class Form references (`Form form = ...`) | Classical polymorphism |
| 12 | Downcasting (`(Book)DataBoundItem`) | Runtime cast |
| 13 | Delegate/Event callbacks | Polymorphic Observer pattern |
| 14 | Boolean dispatch (`if (_isUser) ... else ...`) | Manual branching |
| 15 | JSON serialization (`[JsonDerivedType]` × 5) | Serialization polymorphism |

### 4.4 Delegates & Events in Detail

The codebase uses:

- **2 custom delegate types** (`BookEventHandler`, `CustomerEventHandler`)
- **4 event declarations** (`BookBorrowed`, `BookReturned`, `BookReserved`, `CustomerRegistered`)
- **4 raise sites** (all using `?.Invoke()`)
- **34 event subscriptions** across all forms, including:
  - 3 custom service-event subscriptions (lambda)
  - 13 WinForms `Button.Click` with named methods
  - 8 WinForms `Button.Click` with lambdas
  - 8 other WinForms control events (`TextChanged`, `SelectedIndexChanged`, `CellClick`, `Resize`)
  - 3 local `EventHandler` parameter patterns (`AddButton(string text, EventHandler onClick)`)

### 4.5 JSON Persistence Strategy

The `JsonDataStore` class provides two generic methods:

```csharp
public List<T> Load<T>(string fileName)
public void Save<T>(string fileName, List<T> data)
```

It reads/writes the entire list on every operation — a simple but correct approach for a single-user desktop application with small datasets. The project comment acknowledges this trade-off:

> *"kolan barnamam in bood ke bejaye inke faghat ye bakhshi az data.json ro tagir bedim, kolesh ro barmidarim, taghir ijad mikonim baad kolesh ro az aval toye file data.json zakhire mikonim. ... in harkat konde va ram ziadi mikhore, vali khob sade tar bood."*
> (Translation: "My plan was that instead of only modifying part of data.json, I take the whole thing, make the change, then save the whole thing back. It's slow and uses more RAM, but it was simpler.")

The polymorphic notification hierarchy requires `[JsonDerivedType]` attributes so `System.Text.Json` knows which concrete type to deserialize — without them, serializing `List<Notification>` would lose the derived type information.

---

## 5. Challenges, Diagnostics & Solved Roadblocks

### 5.1 Challenge: Polymorphic JSON Serialization

**Problem:** The notification hierarchy uses abstract base classes. Without configuration, `System.Text.Json` cannot deserialize the correct concrete type — it would create `Notification` objects (which is impossible since it's abstract) or lose the `BookId`, `DueDate` properties of derived types.

**Solution:** Applied `[JsonDerivedType]` attributes on the `Notification` class, one per concrete type:

```csharp
[JsonDerivedType(typeof(OverdueNotification), "OverdueNotification")]
[JsonDerivedType(typeof(ReturnReminderNotification), "ReturnReminderNotification")]
[JsonDerivedType(typeof(ReservationConfirmedNotification), "ReservationConfirmedNotification")]
[JsonDerivedType(typeof(ReservationCancelledNotification), "ReservationCancelledNotification")]
[JsonDerivedType(typeof(BookBorrowedNotification), "BookBorrowedNotification")]
```

This tells the serializer to include a `$type` discriminator in the JSON output, enabling faithful reconstruction.

### 5.2 Challenge: Manual DI without a Container

**Problem:** Without a DI framework, every form must construct its full dependency graph explicitly. This creates repetitive code — for example, `Form1` creates `JsonDataStore`, `JsonCustomerRepository`, `CustomerService`, `JsonUserRepository`, and `UserService` in two different methods (`BtnRegister_Click` and `BtnLogin_Click`).

**Solution:** Accepted as a conscious trade-off. The AGENTS.md documents it explicitly: *"DI is manual: every form creates its own JsonDataStore + repo + service instances inline in its constructor."* This approach keeps the project dependency-free (no NuGet DI packages) and makes the object creation graph fully explicit. The code duplication is manageable for 17 forms.

### 5.3 Challenge: Comment-Out Preservation Convention

**Problem:** The project uses a policy of commenting out old code rather than deleting it. This creates noisy files with legacy code interspersed with active code.

**Solution:** A project-wide convention documented in AGENTS.md: *"Comment-out pattern: old code is commented out, not deleted — preserve this convention."* This allows developers to trace the evolution of the code and revert to previous approaches if needed. Tools like `grep` or `#region` directives could help in the future, but the convention is simple and consistently applied.

### 5.4 Challenge: Extremely Short Loan Due Date

**Problem:** For testing purposes, the due date is set to `DateTime.Now.AddMinutes(2)` — loans are "due" two minutes after borrowing. This would be unusable in production.

**Solution:** Intentionally designed for testing. AGENTS.md documents this as a quirk: *"Loan due in 2 minutes: DueDate = DateTime.Now.AddMinutes(2) in LoanService.BorrowBook — extremely short for testing."* The hardcoded `DailyLateFee = 5000` is also noted as a testing convenience. Both would be moved to configuration in production.

### 5.5 Challenge: No Centralized Notification Trigger

**Problem:** Notification creation was initially scattered across multiple forms, leading to duplication and missed notifications.

**Solution:** Implemented an event-driven architecture where services fire events (`BookBorrowed`, `BookReturned`, `BookReserved`) and the `SeeBooks.cs` form subscribes to these events to create notifications centrally. The `NotificationService.CheckOverdueAndReminders` method provides a batch-processing entry point that can be called periodically (e.g., on application startup or via a timer).

### 5.6 Challenge: Threading & Async Absence

**Problem:** All file I/O is synchronous. Reading/writing JSON on the UI thread can briefly block the interface, though the small dataset size makes this imperceptible.

**Solution:** No async was introduced — a deliberate choice for a single-user desktop app where datasets rarely exceed a few hundred records. If the project scales to networked usage or large datasets, `async Task<List<T>> LoadAsync<T>()` and `async Task SaveAsync<T>()` would be added to `JsonDataStore`.

### 5.7 Challenge: RTL UI Layout

**Problem:** Persian text requires right-to-left layout, which is non-default in Windows Forms.

**Solution:** Every form sets `RightToLeft = RightToLeft.Yes` and `RightToLeftLayout = true`. Fonts use "Vazir" (a Persian-compatible font). Dock and FlowLayoutPanel directions are set to `RightToLeft` throughout. The dark theme (`BackColor = #111520`, accent `#00ff9c`) is applied consistently across all forms.

---

## 6. Future Roadmap & Scalability

### 6.1 Immediate Improvements

| Area | Improvement | Effort |
|------|-------------|--------|
| Due date | Make configurable (appsettings.json or user setting) | Low |
| Late fee | Make configurable instead of hardcoded `5000` | Low |
| JSON concurrency | Add file-locking for multi-instance safety | Medium |
| Password hashing | Replace plain-text with BCrypt or PBKDF2 | Medium |
| Async I/O | Add async overloads to JsonDataStore | Medium |

### 6.2 Feature Expansions

| Feature | Description | Priority |
|---------|-------------|----------|
| **Book cover images** | Store image paths in `Book` model, display in DataGridView | Low |
| **Fines & receipts** | Printable or email-able debt receipts | Low |
| **Barcode/ISBN scanning** | Integrate a barcode scanner for check-in/check-out | Medium |
| **Late-fee grace period** | Configurable days before fines start accruing | Medium |
| **Loan renewal** | Allow customers to extend due date once | Medium |
| **Email/SMS notifications** | Send reminders via external APIs instead of in-app only | High |
| **Reporting dashboard** | Charts: most borrowed books, revenue from fines, active patrons | High |
| **Search indexing** | In-memory search index for faster book lookups | Low |
| **Dark/Light theme toggle** | Persist user preference | Low |

### 6.3 Architectural Scalability

If the system needs to grow beyond single-machine single-user, the following architecture changes would be recommended:

```
Current                 →  Intermediate              →  Full Scale
────────────────────────────────────────────────────────────────
JSON files              →  SQLite                     →  PostgreSQL
Manual DI               →  Microsoft.Extensions.DI    →  Same
WinForms                →  WinForms (same)            →  Blazor Web App
Synchronous I/O         →  async Task everywhere      →  Same
Single-user desktop     →  Network-shared JSON         →  Client-server
In-app notifications    →  Email gateway               →  SMS + Email + Push
```

The interface-based architecture (all services depend on `IRepository<T>` interfaces) makes the JSON-to-SQLite transition straightforward — implement `SqliteBookRepository : IBookRepository` and inject it instead of `JsonBookRepository`. No service layer changes required.

### 6.4 Testing Strategy

The project currently has **zero tests**. A comprehensive testing strategy would include:

- **Unit tests** for `LoanService.ReturnBook` (overdue calculation logic, idempotency) and `NotificationService.CheckOverdueAndReminders`
- **Integration tests** for `JsonRepository<T>` (read/write round-trip)
- **UI smoke tests** for critical flows (register → login → borrow → return → verify debt created)

The abstract `BaseService<T>.Validate()` method is already testable — each service's validation logic can be tested independently.

---

*Document generated from source analysis. All file references are relative to the repository root.*
