# Q&A

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
