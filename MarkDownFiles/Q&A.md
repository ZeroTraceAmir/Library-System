# Q&A

## Q: How does the UI get CustomerId, BookId, LoanDate etc. and how can I change the column text?

**A:** The DataGridView auto-generates columns from the public properties of the `Loan` model class (`Models/Loan.cs:8-12`):

```csharp
public int CustomerId { get; set; }
public int BookId { get; set; }
public DateTime LoanDate { get; set; }
public DateTime DueDate { get; set; }
public DateTime? ReturnDate { get; set; }
```

When `LoadLoans()` sets `dgvLoans.DataSource = _loanService.GetLoansByCustomerId(customer.Id)`, the grid reads these properties and creates a column for each one, using the property name (e.g. `"CustomerId"`) as the column header.

To show Persian/desired headers while keeping the same data, you have two options:

### Option 1 — `[DisplayName]` attribute on the model (simpler)
Add `using System.ComponentModel;` and decorate each property:

```csharp
[DisplayName("آیدی")]
public int Id { get; set; }
[DisplayName("شناسه مشتری")]
public int CustomerId { get; set; }
[DisplayName("شناسه کتاب")]
public int BookId { get; set; }
[DisplayName("تاریخ امانت")]
public DateTime LoanDate { get; set; }
[DisplayName("تاریخ بازگشت")]
public DateTime DueDate { get; set; }
[DisplayName("تاریخ برگردانده شده")]
public DateTime? ReturnDate { get; set; }
```

### Option 2 — Set headers manually after binding (more control)
In `LoadLoans()` after setting `DataSource`:

```csharp
dgvLoans.DataSource = _loanService.GetLoansByCustomerId(customer.Id);

dgvLoans.Columns["Id"].HeaderText = "آیدی";
dgvLoans.Columns["CustomerId"].HeaderText = "شناسه مشتری";
dgvLoans.Columns["BookId"].HeaderText = "شناسه کتاب";
dgvLoans.Columns["LoanDate"].HeaderText = "تاریخ امانت";
dgvLoans.Columns["DueDate"].HeaderText = "تاریخ بازگشت";
dgvLoans.Columns["ReturnDate"].HeaderText = "تاریخ برگردانده شده";
```

### Which to use?
- **Option 1** is cleaner if you always want the same headers everywhere `Loan` is displayed.
- **Option 2** is better if different forms need different header text for the same model.

---

## Q: In `LoanService.cs`, what does `BookReturned?.Invoke(book)` mean? Which events are being called when a book is returned?

**A:** `BookReturned?.Invoke(book)` fires the `BookReturned` event (a `BookEventHandler` delegate), passing the returned `Book` object to any subscribed handlers.

There are two events defined in `LoanService`:

| Event | Fired from method | When |
|---|---|---|
| `BookBorrowed` | `BorrowBook()` | After a book is successfully borrowed |
| `BookReturned` | `ReturnBook()` | After a book is successfully returned, `CopiesAvailable` is incremented, and the loan is updated |

However, **neither event is currently subscribed to anywhere in the codebase**. Searching `+= BookReturned` and `+= BookBorrowed` across all `.cs` files returns no results. The events are fired (the `?.Invoke(...)` pattern safely does nothing if no handler is attached), but no form or service listens to them. They appear to be scaffolding for future features (e.g., showing notifications, updating UI in real-time).

---

## Q: Should I create different notification services for different notification types?

**Context (from your answers):**
- Notifications are auto-generated from `LoanService` events (`BookBorrowed`, `BookReturned`)
- Only customers view them (in the Home page)
- Each notification type has different creation logic

**Answer:** A **single `NotificationService` with dedicated methods per type** is the better approach here. Reasons:

| Approach | Pros | Cons |
|---|---|---|
| **Single service** | — One repository, one pattern<br>— Shared logic (e.g., saving, fetching for a customer)<br>— Fewer files, easier to navigate | Class grows as types increase |
| **Separate services** | — Follows Single Responsibility strictly | — Too many small classes<br>— Duplicated repo/query logic<br>— Over-engineering for a project this size |

Since all notifications share the same repository, the same audience (customers), and the same display surface (`Notifications` form), a single service with clearly-named methods is cleaner:

```
public class NotificationService
{
    public void CreateOverdueNotification(int customerId, int bookId, DateTime dueDate) { ... }
    public void CreateReturnReminderNotification(int customerId, int bookId, DateTime dueDate) { ... }
    public void CreateReservationConfirmedNotification(int customerId, int bookId) { ... }
    public List<Notification> GetNotificationsForCustomer(int customerId) { ... }
}
```

If a type ever grows complex enough to warrant its own class, you can extract it later — no need to pre-split.
