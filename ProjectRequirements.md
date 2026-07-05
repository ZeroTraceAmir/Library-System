# Library System — Project Requirements

## Part 1: Polymorphism

This section catalogues every polymorphic pattern used across the codebase, organized by mechanism.

---

### 1. Interface Polymorphism

#### Entity Interface Hierarchy

All domain models implement `IEntity` (or an interface that extends it), allowing uniform identity handling.

| Interface | File | Line | Extends |
|-----------|------|------|---------|
| `IEntity` | `Interfaces/IEntity.cs` | 8 | *(none)* &nbsp;—&nbsp; `int Id { get; set; }` |
| `IBook` | `Interfaces/IBook.cs` | 3 | `IEntity` |
| `ICustomer` | `Interfaces/ICustomer.cs` | 3 | `IEntity` |
| `IDebt` | `Interfaces/IDebt.cs` | 5 | `IEntity` |
| `ILoan` | `Interfaces/ILoan.cs` | 5 | `IEntity` |
| `IReservation` | `Interfaces/IReservation.cs` | 5 | `IEntity` |
| `IUser` | `Interfaces/IUser.cs` | 5 | *(none — declares its own `Id`)* |

#### Repository Interface Hierarchy

`IRepository<T>` is constrained to `IEntity`, and each entity type has a dedicated sub-interface.

```
IRepository<T> where T : IEntity       ← Interfaces/IRepository.cs:8
 ├── IBookRepository : IRepository<Book>
 ├── ICustomerRepository : IRepository<Customer>
 ├── IDebtRepository : IRepository<Debt>
 ├── ILoanRepository : IRepository<Loan>
 ├── INotificationRepository : IRepository<Notification>
 ├── IReservationRepository : IRepository<Reservation>
 └── IUserRepository : IRepository<User>
```

This allows any repository to be referenced by its base interface, enabling polymorphic substitution.

---

### 2. Abstract Class Inheritance Hierarchy

```
BaseEntity (abstract) : IEntity                      Models/BaseEntity.cs:5
 └── Person (abstract)                               Models/Person.cs:3
      └── Account (abstract, virtual GetRoleLabel()) Models/Account.cs:3
           ├── Customer : ICustomer                  Models/Customer.cs
           └── User : IUser                          Models/User.cs
 └── Book : IBook                                    Models/Book.cs
 └── Debt : IDebt                                    Models/Debt.cs
 └── Loan : ILoan                                    Models/Loan.cs
 └── Reservation : IReservation                      Models/Reservation.cs
 └── Notification (abstract, abstract GetMessage())  Models/Notification.cs:12
      └── BookNotification (abstract)                Models/BookNotification.cs:8
           ├── BookBorrowedNotification               Models/BookBorrowedNotification.cs
           ├── ReservationCancelledNotification        Models/ReservationCancelledNotification.cs
           ├── ReservationConfirmedNotification        Models/ReservationConfirmedNotification.cs
           └── LoanNotification (abstract)            Models/LoanNotification.cs:8
                ├── OverdueNotification                Models/OverdueNotification.cs
                └── ReturnReminderNotification         Models/ReturnReminderNotification.cs
```

The entire model layer is built on polymorphic inheritance — every entity is substitutable for `BaseEntity`, every person for `Person`, every account for `Account`, and every notification for `Notification`.

#### Repository + Service inheritance

```
JsonRepository<T> : IRepository<T>  where T : IEntity     Repositories/JsonRepository.cs:9
 ├── JsonBookRepository     : JsonRepository<Book>,     IBookRepository
 ├── JsonCustomerRepository : JsonRepository<Customer>, ICustomerRepository
 ├── JsonDebtRepository     : JsonRepository<Debt>,     IDebtRepository
 ├── JsonLoanRepository     : JsonRepository<Loan>,     ILoanRepository
 ├── JsonNotificationRepo   : JsonRepository<Notification>, INotificationRepository
 ├── JsonReservationRepo    : JsonRepository<Reservation>, IReservationRepository
 └── JsonUserRepository     : JsonRepository<User>,     IUserRepository

BaseService<T> (abstract)                                    Services/BaseService.cs:3
 ├── BookService     : BaseService<Book>
 ├── CustomerService : BaseService<Customer>
 └── UserService     : BaseService<User>
```

---

### 3. Virtual Method Dispatch (Runtime Polymorphism)

#### `Account.GetRoleLabel()`

The base class provides a default; each subclass overrides it to return a role-specific label.

| File | Line | Member |
|------|------|--------|
| `Models/Account.cs` | 9 | `public virtual string GetRoleLabel() => "کاربر";` |
| `Models/Customer.cs` | 11 | `public override string GetRoleLabel() => "مشتری";` |
| `Models/User.cs` | 10 | `public override string GetRoleLabel() => Role == UserStatus.admin ? "مدیر" : "کارمند";` |

**Call sites:**

| File | Line | Code |
|------|------|------|
| `Home.cs` | 18 | `customer.GetRoleLabel()` |
| `AdminPanel.cs` | 18 | `user.GetRoleLabel()` |
| `StaffPanel.cs` | 18 | `user.GetRoleLabel()` |

The correct implementation is resolved at runtime based on the actual object type.

---

### 4. Abstract Method Polymorphism

#### `Notification.GetMessage()`

The abstract base forces every concrete notification to provide its own message text.

```csharp
// Models/Notification.cs:17
public abstract string GetMessage();
```

| Override | File | Line | Returns |
|----------|------|------|---------|
| `BookBorrowedNotification.GetMessage()` | `Models/BookBorrowedNotification.cs` | 7 | `"کتاب با موفقیت امانت گرفته شد."` |
| `OverdueNotification.GetMessage()` | `Models/OverdueNotification.cs` | 10 | `"کتاب شما دیر شده است."` |
| `ReservationCancelledNotification.GetMessage()` | `Models/ReservationCancelledNotification.cs` | 7 | `"رزرو کتاب شما لغو شد."` |
| `ReservationConfirmedNotification.GetMessage()` | `Models/ReservationConfirmedNotification.cs` | 7 | `"رزرو کتاب شما تایید شد."` |
| `ReturnReminderNotification.GetMessage()` | `Models/ReturnReminderNotification.cs` | 10 | `"مهلت بازگرداندن کتاب نزدیک است."` |

**Polymorphic dispatch at runtime:**

```csharp
// Notifications.cs:73-76, 113-115
foreach (Notification notification in notifications)     // base-type collection
{
    messageLabel.Text = notification.GetMessage();        // ← runtime dispatch
}
```

#### `BaseService<T>.Validate(T)`

```csharp
// Services/BaseService.cs:5
protected abstract void Validate(T entity);
```

| Override | File | Line |
|----------|------|------|
| `BookService.Validate(Book)` | `Services/BookService.cs` | 190 |
| `CustomerService.Validate(Customer)` | `Services/CustomerService.cs` | 90 |
| `UserService.Validate(User)` | `Services/UserService.cs` | 137 |

Each service provides entity-specific validation logic while sharing the same abstract contract.

---

### 5. Generic Polymorphism

#### Constrained Generics

```csharp
// Interfaces/IRepository.cs:8
public interface IRepository<T> where T : IEntity

// Repositories/JsonRepository.cs:9-10
public class JsonRepository<T> : IRepository<T> where T : IEntity
```

The `where T : IEntity` constraint guarantees that `T` has an `Id` property, which `JsonRepository<T>` uses in its `GetById`, `Update`, and `Delete` methods (lines 30, 44, 56).

#### Unconstrained Generics

```csharp
// Repositories/JsonDataStore.cs:14, 26
public List<T> Load<T>()    // works on any List<T>
public void Save<T>()        // works on any List<T>
```

`JsonDataStore` provides polymorphic load/save for any type without requiring an interface constraint.

---

### 6. Polymorphic Collections & `is` Pattern Matching

#### Type-checking with declaration patterns

In `NotificationService` (`Services/NotificationService.cs`):

```csharp
// Lines 100-105
n is OverdueNotification
    && n.CustomerId == loan.CustomerId
    && n is BookNotification bn       // declaration pattern — captures typed variable
    && bn.BookId == loan.BookId

// Lines 113-117
n is ReturnReminderNotification
    && n.CustomerId == loan.CustomerId
    && n is BookNotification bn2      // declaration pattern
    && bn2.BookId == loan.BookId
```

#### Downcasting from `object`

```csharp
// SeeBooks.cs:136, 158
Book book = (Book)dgvBooks.CurrentRow.DataBoundItem;
```

#### Enum casting

| File | Line | Cast |
|------|------|------|
| `SeeAllCustomers.cs` | 105 | `(CustomerFilter)_cmbFilter.SelectedIndex` |
| `SeeAllEmployees.cs` | 175 | `(UserFilter)_cmbFilter.SelectedIndex` |
| `SeeAllEmployees.cs` | 80 | `(int)u.Role` |
| `AddEmployee.cs` | 80 | `(UserStatus)cmbRole.SelectedIndex` |

---

### 7. Polymorphic Form References (Base-Class Variable)

The `Form` base class is used to hold derived form instances.

```csharp
// Program.cs:69-71
Form form = UserLoggedIn.Role == Enums.UserStatus.admin
    ? new AdminPanel(UserLoggedIn)
    : new StaffPanel(UserLoggedIn);
Application.Run(form);

// Form1.cs:143-158
Form nextForm;
if (loginForm.LoggedInUserRole == Enums.UserStatus.admin)
    nextForm = new AdminPanel(user);
else if (loginForm.LoggedInUserRole == Enums.UserStatus.staff)
    nextForm = new StaffPanel(user);
else
    nextForm = new Home(customer);
nextForm.Show();
```

---

### 8. Delegate / Event Polymorphism

*(See Part 2 for the complete inventory.)*

#### Delegate types

```csharp
// Services/CustomerService.cs:11
public delegate void CustomerEventHandler(Customer customer);

// Services/LoanService.cs:9
public delegate void BookEventHandler(Book book, Loan? loan, int customerId);
```

#### Events

| Event | File | Line | Delegate Type |
|-------|------|------|---------------|
| `CustomerService.CustomerRegistered` | `Services/CustomerService.cs` | 18 | `CustomerEventHandler` |
| `LoanService.BookBorrowed` | `Services/LoanService.cs` | 19 | `BookEventHandler` |
| `LoanService.BookReturned` | `Services/LoanService.cs` | 20 | `BookEventHandler` |
| `ReservationService.BookReserved` | `Services/ReservationService.cs` | 50 | `BookEventHandler` |

#### Lambda subscribers

```csharp
// SeeBooks.cs:48-52
reservationService.BookReserved += (book, _, customerId) =>
    notificationService.CreateReservationConfirmedNotification(customerId, book.Id);

loanService.BookBorrowed += (book, _, customerId) =>
    notificationService.CreateBookBorrowedNotification(customerId, book.Id);
```

#### `EventHandler` as local function parameter

`AdminPanel.cs:75`, `StaffPanel.cs:75`, `Home.cs:192` — each defines a local `void AddButton(string text, EventHandler onClick)` that accepts any `EventHandler` delegate and attaches it to the button's `Click` event.

#### Boolean-based manual dispatch (simulated polymorphism)

```csharp
// Profile.cs:90-93
if (_isUser)
    _userService.Logout(_phone);
else
    _customerService.Logout(_phone);

// Profile.cs:193-196
if (_isUser)
    _userService.UserProfileEdit(name, phone, password, _phone);
else
    _customerService.CustomerProfileEdit(name, phone, password, _phone);
```

---

### 9. JSON Serialization Polymorphism

`[JsonDerivedType]` attributes on `Notification` enable `System.Text.Json` to serialize and deserialize concrete notification types through their abstract base:

```csharp
// Models/Notification.cs:7-11
[JsonDerivedType(typeof(OverdueNotification), "OverdueNotification")]
[JsonDerivedType(typeof(ReturnReminderNotification), "ReturnReminderNotification")]
[JsonDerivedType(typeof(ReservationConfirmedNotification), "ReservationConfirmedNotification")]
[JsonDerivedType(typeof(ReservationCancelledNotification), "ReservationCancelledNotification")]
[JsonDerivedType(typeof(BookBorrowedNotification), "BookBorrowedNotification")]
```

---

### Summary Table (Polymorphism)

| # | Pattern | Key Types / Locations | Mechanism |
|---|---------|----------------------|-----------|
| 1 | Interface inheritance | `IEntity`, `IBook`, `ICustomer`, `ILoan`, `IDebt`, `IReservation`, `IUser` | Interface-to-interface extension |
| 2 | Repository interfaces | `IRepository<T>`, 7 sub-interfaces | Generic constraint + interface inheritance |
| 3 | Abstract inheritance | `BaseEntity` → `Person` → `Account` → `Customer`/`User` | Multi-level abstract chain |
| 4 | Notification hierarchy | `Notification` → `BookNotification` → 5 concrete types | Abstract chain with 3 levels |
| 5 | Virtual method | `Account.GetRoleLabel()` → overrides in `Customer`, `User` | Runtime virtual dispatch |
| 6 | Abstract method | `Notification.GetMessage()` → 5 overrides | Runtime dispatch on `Notification` variable |
| 7 | Abstract method (generic) | `BaseService<T>.Validate(T)` → 3 overrides | Runtime dispatch by entity type |
| 8 | Generic constraint | `IRepository<T> where T : IEntity` | Compile-time polymorphism |
| 9 | Generic methods | `JsonDataStore.Load<T>()` / `Save<T>()` | Unconstrained generics |
| 10 | `is` pattern matching | `NotificationService.cs` — `n is OverdueNotification`, `n is BookNotification bn` | Runtime type inspection |
| 11 | Base-class Form ref | `Program.cs:69`, `Form1.cs:143` — `Form` variable holds derived forms | Classical polymorphism |
| 12 | Downcast from `object` | `SeeBooks.cs:136,158` — `(Book)DataBoundItem` | Runtime cast |
| 13 | Delegate/Event | `BookEventHandler`, `CustomerEventHandler` + lambda subscribers | Polymorphic callbacks |
| 14 | Bool dispatch | `Profile.cs:90,193` — `if (_isUser) ... else ...` | Manual branching |
| 15 | JSON poly | `[JsonDerivedType]` × 5 on `Notification` | Serialization polymorphism |

---

## Part 2: Delegates & Events — Complete Inventory

This section catalogues every delegate and event usage in the codebase.

---

### 1. Delegate Type Declarations

| # | File | Line | Declaration |
|---|------|------|-------------|
| 1 | `Services/LoanService.cs` | 9 | `public delegate void BookEventHandler(Book book, Loan? loan, int customerId);` |
| 2 | `Services/CustomerService.cs` | 11 | `public delegate void CustomerEventHandler(Customer customer);` |

---

### 2. Event Declarations

| # | File | Line | Event | Delegate Type |
|---|------|------|-------|---------------|
| 1 | `Services/LoanService.cs` | 19 | `event BookEventHandler? BookBorrowed` | `BookEventHandler` |
| 2 | `Services/LoanService.cs` | 20 | `event BookEventHandler? BookReturned` | `BookEventHandler` |
| 3 | `Services/ReservationService.cs` | 50 | `event BookEventHandler? BookReserved` | `BookEventHandler` |
| 4 | `Services/CustomerService.cs` | 18 | `event CustomerEventHandler? CustomerRegistered` | `CustomerEventHandler` |

---

### 3. Event Invocations (Raise Sites)

| # | File | Line | Invocation |
|---|------|------|------------|
| 1 | `Services/LoanService.cs` | 178 | `BookBorrowed?.Invoke(book, loan, customerId);` |
| 2 | `Services/LoanService.cs` | 157 | `BookReturned?.Invoke(book, loan, loan.CustomerId);` |
| 3 | `Services/ReservationService.cs` | 67 | `BookReserved?.Invoke(book, null, customerId);` |
| 4 | `Services/CustomerService.cs` | 36 | `CustomerRegistered?.Invoke(customer);` |

All use the null-conditional `?.Invoke()` pattern.

---

### 4. Event Subscriptions (`+=`) — Custom Service Events

| # | File | Line | Subscription |
|---|------|------|-------------|
| 1 | `SeeBooks.cs` | 48–49 | `reservationService.BookReserved += (book, _, customerId) => notificationService.CreateReservationConfirmedNotification(customerId, book.Id);` |
| 2 | `SeeBooks.cs` | 51–52 | `loanService.BookBorrowed += (book, _, customerId) => notificationService.CreateBookBorrowedNotification(customerId, book.Id);` |
| 3 | `RegistrationForm.cs` | 21–26 | `customerService.CustomerRegistered += c => ShowMessage($"خوش آمدید {c.Name}! ثبت نام با موفقیت انجام شد", "موفق", MessageBoxIcon.Information);` |

---

### 5. Event Subscriptions (`+=`) — WinForms Button.Click (Named Methods)

| # | File | Line | Subscription | Handler (File:Line) |
|---|------|------|-------------|---------------------|
| 1 | `Form1.cs` | 81 | `btnRegister.Click += BtnRegister_Click;` | `Form1.cs:122` |
| 2 | `Form1.cs` | 110 | `btnLogin.Click += BtnLogin_Click;` | `Form1.cs:132` |
| 3 | `LoginForm.cs` | 134 | `btnLogin.Click += BtnLogin_Click;` | `LoginForm.cs:158` |
| 4 | `LoginForm.cs` | 146 | `btnBack.Click += BtnBack_Click;` | `LoginForm.cs:199` |
| 5 | `Home.cs` | 42 | `btnProfile.Click += BtnProfile_Click;` | `Home.cs:258` |
| 6 | `AdminPanel.cs` | 42 | `btnProfile.Click += BtnProfile_Click;` | `AdminPanel.cs:122` |
| 7 | `StaffPanel.cs` | 42 | `btnProfile.Click += BtnProfile_Click;` | `StaffPanel.cs:122` |
| 8 | `SeeBooks.cs` | 116 | `btnBorrow.Click += BtnBorrow_Click;` | `SeeBooks.cs:131` |
| 9 | `SeeBooks.cs` | 117 | `btnReserve.Click += BtnReserve_Click;` | `SeeBooks.cs:153` |
| 10 | `MyLoans.cs` | 57 | `btnReturn.Click += BtnReturn_Click;` | `MyLoans.cs:120` |
| 11 | `MyReservations.cs` | 51 | `btnCancelReservation.Click += BtnCancelReservation_Click;` | `MyReservations.cs:100` |
| 12 | `PayDebt.cs` | 54 | `btnPay.Click += BtnPay_Click;` | `PayDebt.cs:113` |
| 13 | `RegistrationForm.cs` | 78 | `btnRegister.Click += BtnRegister_Click;` | `RegistrationForm.cs:103` |

---

### 6. Event Subscriptions (`+=`) — WinForms Button.Click (Lambda)

| # | File | Line | Lambda Body |
|---|------|------|-------------|
| 1 | `Form1.cs` | 100–108 | `(s, e) =>` reposition buttons on panel resize |
| 2 | `Home.cs` | 59 | `(s, e) => new Notifications(_customer).ShowDialog(this);` |
| 3 | `Profile.cs` | 88–106 | `(s, e) =>` logout logic |
| 4 | `Profile.cs` | 161–203 | `(s, e) =>` save profile fields |
| 5 | `Profile.cs` | 221–225 | `(s, e) =>` delete account |
| 6 | `Profile.cs` | 239 | `(s, e) => this.Close();` |
| 7 | `AddBook.cs` | 64–107 | `(s, e) =>` add book form logic |
| 8 | `AddEmployee.cs` | 72–107 | `(s, e) =>` add employee form logic |

---

### 7. Event Subscriptions (`+=`) — WinForms Control Events (Lambda)

| # | File | Line | Control & Event | Lambda |
|---|------|------|-----------------|--------|
| 1 | `SeeAllBooks.cs` | 75 | `_txtSearch.TextChanged +=` | `(s, e) => RefreshGrid();` |
| 2 | `SeeAllCustomers.cs` | 41 | `_txtSearch.TextChanged +=` | `(s, e) => RefreshGrid();` |
| 3 | `SeeAllEmployees.cs` | 44 | `_txtSearch.TextChanged +=` | `(s, e) => RefreshGrid();` |
| 4 | `SeeAllBooks.cs` | 88 | `_cmbFilter.SelectedIndexChanged +=` | `(s, e) => RefreshGrid();` |
| 5 | `SeeAllCustomers.cs` | 53 | `_cmbFilter.SelectedIndexChanged +=` | `(s, e) => RefreshGrid();` |
| 6 | `SeeAllEmployees.cs` | 56 | `_cmbFilter.SelectedIndexChanged +=` | `(s, e) => RefreshGrid();` |
| 7 | `SeeAllEmployees.cs` | 107 | `_grid.CellClick +=` | `OnGridCellClick` (named) |
| 8 | `Form1.cs` | 100 | `buttonPanel.Resize +=` | `(s, e) =>` reposition buttons |

---

### 8. `EventHandler` as Local Function Parameter

Three panels use a local `AddButton` function that accepts an `EventHandler` delegate, enabling polymorphic button creation:

| File | Line | Signature |
|------|------|-----------|
| `AdminPanel.cs` | 75 | `void AddButton(string text, EventHandler onClick)` |
| `StaffPanel.cs` | 75 | `void AddButton(string text, EventHandler onClick)` |
| `Home.cs` | 192 | `void AddButton(string text, EventHandler onClick)` |

Each calls `btn.Click += onClick;` to attach the delegate, then is invoked with lambdas:

```csharp
// AdminPanel.cs:96-113
AddButton("دیدن همه کارمندان کتابخانه", (s, e) => { ... });
AddButton("دیدن همه مشتریان کتابخانه", (s, e) => { ... });
AddButton("اضافه کردن کارمند به کتاب خانه", (s, e) => { ... });
```

---

### 9. Named Event Handler Methods

| Handler | File | Line |
|---------|------|------|
| `BtnRegister_Click` | `Form1.cs` | 122 |
| `BtnLogin_Click` | `Form1.cs` | 132 |
| `BtnLogin_Click` | `LoginForm.cs` | 158 |
| `BtnBack_Click` | `LoginForm.cs` | 199 |
| `BtnProfile_Click` | `Home.cs` | 258 |
| `BtnProfile_Click` | `AdminPanel.cs` | 122 |
| `BtnProfile_Click` | `StaffPanel.cs` | 122 |
| `BtnBorrow_Click` | `SeeBooks.cs` | 131 |
| `BtnReserve_Click` | `SeeBooks.cs` | 153 |
| `BtnReturn_Click` | `MyLoans.cs` | 120 |
| `BtnCancelReservation_Click` | `MyReservations.cs` | 100 |
| `BtnPay_Click` | `PayDebt.cs` | 113 |
| `BtnRegister_Click` | `RegistrationForm.cs` | 103 |
| `OnGridCellClick` | `SeeAllEmployees.cs` | 128 |

---

### Summary Statistics (Delegates & Events)

| Category | Count |
|----------|-------|
| Delegate type declarations | 2 |
| Event declarations | 4 |
| Event invocations (`?.Invoke()`) | 4 |
| Custom service event subscriptions | 3 |
| WinForms Button.Click subscriptions (named) | 13 |
| WinForms Button.Click subscriptions (lambda) | 8 |
| WinForms other event subscriptions | 8 |
| `EventHandler` local function params | 3 |
| Named handler methods | 14 (9 unique names) |
