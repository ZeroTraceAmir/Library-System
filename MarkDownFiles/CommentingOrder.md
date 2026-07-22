# Commenting Order — Bottom-Up Dependency Walk

Start from the leaves (no dependencies) and work up. Never comment a file that references types you haven't documented yet.

---

### Phase 1: Foundation (Models & Contracts)

| Step | File | Why start here |
|------|------|----------------|
| 1 | `Models/BaseEntity.cs` | Simplest file in the project — root of the entire model hierarchy. ~4 lines of real code. |
| 2 | `Interfaces/IEntity.cs` | The contract `BaseEntity` implements. 1 property. |
| 3 | `Models/Person.cs` | Adds `Name`. Inherits `BaseEntity`. First encounter with inheritance. |
| 4 | `Models/Account.cs` | Adds `Number`, `Password`, `IsLogedin`, and `virtual` method `GetRoleLabel()`. First meaningful polymorphism. |
| 5 | `Models/Customer.cs` | First `override` of `GetRoleLabel()`. Multi-level inheritance chain complete. |
| 6 | `Models/User.cs` | Second `override`, plus `Role` enum property. |
| 7 | `Models/Book.cs` | Standalone entity — properties map to `IBook`. |
| 8 | `Models/Loan.cs` | Introduces `DateTime?` nullable pattern. |
| 9 | `Models/Reservation.cs` | Same pattern as Loan. |
| 10 | `Models/Debt.cs` | Same pattern, introduces `decimal` type. |

---

### Phase 2: Enums & Interfaces

| Step | File | Why here |
|------|------|----------|
| 11 | `Enums/UserStatus.cs` | Zero dependencies. Used everywhere. Quick win. |
| 12 | `Enums/NotificationType.cs` | Zero dependencies. |
| 13 | `Enums/CustomerFilter.cs` | Zero dependencies. |
| 14 | `Enums/UserFilter.cs` | Zero dependencies. |
| 15 | `Interfaces/IEntity.cs` → `IUserRepository.cs` (13 files) | All are read-only contracts — property/method signatures only. Comment as a batch. |

---

### Phase 3: Notification Hierarchy

| Step | File | Why here |
|------|------|----------|
| 16 | `Models/Notification.cs` | Introduces `abstract` method + `[JsonDerivedType]` attribute. |
| 17 | `Models/BookNotification.cs` | Abstract intermediate — adds `BookId`. |
| 18 | `Models/LoanNotification.cs` | Abstract intermediate — adds `DueDate`. |
| 19 | `Models/BookBorrowedNotification.cs` | Concrete — single `GetMessage()` override. |
| 20 | `Models/OverdueNotification.cs` | Concrete — single `GetMessage()` override. |
| 21 | `Models/ReservationCancelledNotification.cs` | Concrete — single `GetMessage()` override. |
| 22 | `Models/ReservationConfirmedNotification.cs` | Concrete — single `GetMessage()` override. |
| 23 | `Models/ReturnReminderNotification.cs` | Concrete — single `GetMessage()` override. |

---

### Phase 4: Data Access Layer

| Step | File | Why here |
|------|------|----------|
| 24 | `Repositories/JsonDataStore.cs` | The persistence engine — generic `Load<T>()` / `Save<T>()`. Comments explain the entire JSON strategy. |
| 25 | `Repositories/JsonRepository.cs` | Generic CRUD implementation. The most important repository — all others inherit from it. |
| 26 | `Repositories/JsonBookRepository.cs` | ~10 lines — just constructor passing file name. |
| 27 | `Repositories/JsonCustomerRepository.cs` | Same pattern. |
| 28 | `Repositories/JsonUserRepository.cs` | Same pattern. |
| 29 | `Repositories/JsonLoanRepository.cs` | Same pattern. |
| 30 | `Repositories/JsonReservationRepository.cs` | Same pattern. |
| 31 | `Repositories/JsonDebtRepository.cs` | Same pattern. |
| 32 | `Repositories/JsonNotificationRepository.cs` | Same pattern. |

---

### Phase 5: Business Logic Layer (Services)

| Step | File | Why here |
|------|------|----------|
| 33 | `Services/BaseService.cs` | Abstract service root. ~6 lines. |
| 34 | `Services/BookService.cs` | Most complete service — search, filter, validation, indexer. Template for all others. |
| 35 | `Services/CustomerService.cs` | Adds events (`CustomerRegistered`), login/logout logic. |
| 36 | `Services/UserService.cs` | Adds role management (`UserStatus`). |
| 37 | `Services/LoanService.cs` | Core borrow/return logic + events. First standalone service (no `BaseService`). |
| 38 | `Services/ReservationService.cs` | Simple standalone service. |
| 39 | `Services/DebtService.cs` | Simple standalone service. |
| 40 | `Services/NotificationService.cs` | `is` pattern matching + `CheckOverdueAndReminders` — most interesting business logic in the project. |

---

### Phase 6: Application Entry Point

| Step | File | Why here |
|------|------|----------|
| 41 | `Program.cs` | Entry point. At this point every referenced type is already documented. |

---

### Phase 7: Presentation Layer (Forms)

Now you know every model, service, and repository that the forms use.

| Step | File | Role |
|------|------|------|
| 42 | `Form1.cs` | Gateway — login/register entry screen. |
| 43 | `LoginForm.cs` | Authentication for all 3 roles. |
| 44 | `RegistrationForm.cs` | Customer registration with event subscription. |
| 45 | `Home.cs` | Customer dashboard. |
| 46 | `AdminPanel.cs` | Admin dashboard. |
| 47 | `StaffPanel.cs` | Staff dashboard. |
| 48 | `SeeBooks.cs` | Customer book browsing + borrow/reserve. |
| 49 | `SeeAllBooks.cs` | Staff book management with search/filter. |
| 50 | `SeeAllCustomers.cs` | Staff customer list with filter. |
| 51 | `SeeAllEmployees.cs` | Admin employee list with delete. |
| 52 | `MyLoans.cs` | Customer active loans + return. |
| 53 | `MyReservations.cs` | Customer reservations + cancel. |
| 54 | `PayDebt.cs` | Customer debt payment. |
| 55 | `Notifications.cs` | Notification display with polymorphic `GetMessage()`. |
| 56 | `Profile.cs` | Profile view/edit with bool-based dispatch. |
| 57 | `AddBook.cs` | Book creation form. |
| 58 | `AddEmployee.cs` | Employee creation form. |

---

### Summary

| Phase | Files | Cumulative |
|-------|-------|------------|
| 1. Foundation Models | 10 | 10 |
| 2. Enums & Interfaces | 17 | 27 |
| 3. Notification Hierarchy | 8 | 35 |
| 4. Data Access Layer | 9 | 44 |
| 5. Services | 8 | 52 |
| 6. Entry Point | 1 | 53 |
| 7. Forms | 17 | 70 |

**~70 files** total (some interfaces and enums are small enough to batch per step).
