# Return Logic — How Book Returning Works

## Overview

The return feature allows a customer to return a borrowed book through the `MyLoans` form. The flow follows a strict layered architecture:

```
MyLoans (UI Form)
    ↓  calls services
LoanService / BookService (Business Logic)
    ↓  calls repositories
JsonLoanRepository / JsonBookRepository (Data Access)
    ↓  reads/writes JSON files
JsonDataStore → Data/*.json (Persistence)
```

---

## 1. UI Layer — `MyLoans.cs`

### Entry point: `BtnReturn_Click` (line 99)

```csharp
private void BtnReturn_Click(object? sender, EventArgs e)
```

**Step-by-step:**

1. **Validate selection** — Checks `dgvLoans.CurrentRow` is not null (user must select a row). Shows `"ابتدا یک امانت را انتخاب کنید"` if nothing selected.

2. **Extract loan ID** — Reads the `Id` cell from the selected row. The `Id` column exists in the data source (`LoanDisplay` record) but is hidden (`Visible = false`). The value is still accessible:
   ```csharp
   int loanId = Convert.ToInt32(dgvLoans.CurrentRow.Cells["Id"].Value);
   ```

3. **Fetch Loan entity** — Calls `_loanService.GetLoanById(loanId)` to get the full `Loan` object from the repository. Returns null if not found (guard clause).

4. **Fetch Book entity** — Calls `_bookService.GetBookById(loan.BookId)` using the loan's `BookId` to get the associated `Book` object. Returns null if not found (guard clause).

5. **Execute return** — Calls `_loanService.ReturnBook(loanId, book)` which handles the core return logic.

6. **Persist book change** — Calls `_bookService.UpdateBook(book)`. Even though `ReturnBook` already incremented `book.CopiesAvailable` in memory, the book object must be explicitly saved to the repository.

7. **Refresh grid** — Calls `LoadLoans()` to re-fetch and re-bind the loan data (shows updated state).

8. **Success message** — Shows `"کتاب با موفقیت برگردانده شد"`.

---

## 2. Service Layer — `LoanService.cs`

### `ReturnBook(int loanId, Book book)` (line 56)

```csharp
public void ReturnBook(int loanId, Book book)
```

This method **is NOT in `BookService`** — it lives in `LoanService` because the primary operation is updating a loan record; the book update is a side effect.

**What it does internally:**

```csharp
// 1. Fetch the loan from the repository
Loan? loan = loanRepository.GetById(loanId);
if (loan == null)
    throw new Exception("امانت مورد نظر یافت نشد");

// 2. Set the return date to now (marks the loan as returned)
loan.ReturnDate = DateTime.Now;

// 3. Increment the book's available copies
book.CopiesAvailable++;

// 4. Persist the loan changes
loanRepository.Update(loan);

// 5. Fire the BookReturned event (currently no subscribers)
BookReturned?.Invoke(book, loan, loan.CustomerId);
```

**Key design decisions:**

| Concern | How it's handled |
|---|---|
| **Loan update** | `loan.ReturnDate = DateTime.Now` — the `DueDate` is NOT modified |
| **Book availability** | `book.CopiesAvailable++` — increments in memory only; the caller `MyLoans` must call `_bookService.UpdateBook(book)` to persist |
| **Event notification** | `BookReturned?.Invoke(...)` fires with `(book, loan, customerId)`. The event has **zero subscribers** in the current codebase — it exists as future scaffolding |
| **Debt/fine calculation** | **Not implemented** — there is no automatic late-fee calculation when `ReturnDate > DueDate`. The `DebtService` and `PayDebt` form exist but debts must be created manually |
| **Validation** | Only checks if loan exists. No check if the loan is already returned (`ReturnDate != null`). Calling `ReturnBook` twice would overwrite the first return date |

### Why `book.CopiesAvailable++` is not saved here

The `LoanService` only has access to `ILoanRepository`. It does not know about `IBookRepository` (no dependency injection of book repo into `LoanService`). So it can only modify the `Book` object in memory. Saving the book is delegated to the caller via `_bookService.UpdateBook(book)`.

This is a **design gap** — if a developer forgets to call `UpdateBook`, the book's `CopiesAvailable` becomes out of sync with reality.

---

## 3. Service Layer — `BookService.cs`

### `GetBookById(int id)` and `UpdateBook(Book book)`

```csharp
public Book? GetBookById(int id)
{
    return bookRepository.GetById(id);
}

public void UpdateBook(Book book)
{
    Validate(book);       // validates Title, Author, etc.
    bookRepository.Update(book);
}
```

`UpdateBook` runs validation before persisting — but since we're only changing `CopiesAvailable`, the validation always passes.

---

## 4. Repository Layer — `JsonRepository<T>` (base class)

All repositories inherit from `JsonRepository<T>` which provides the CRUD implementation:

### `Update(T entity)` (JsonRepository.cs:41)

```csharp
public void Update(T entity)
{
    List<T> items = GetAll();        // Read entire JSON file
    int index = items.FindIndex(x => x.Id == entity.Id);

    if (index != -1)
    {
        items[index] = entity;        // Replace in-memory
        dataStore.Save(filePath, items);  // Write entire file back
    }
}
```

**Performance note:** The entire JSON file is read into memory, modified, and written back. This is fine for this project's scale (small JSON files) but would not scale to thousands of records.

### `GetById(int id)` (JsonRepository.cs:27)

```csharp
public T? GetById(int id)
{
    List<T>? items = GetAll();           // Read entire JSON file
    T item = items.FirstOrDefault(x => x.Id == id);
    return item;                          // Returns null if not found
}
```

---

## 5. Data Layer — `JsonDataStore.cs`

### `Load<T>(string fileName)` and `Save<T>(string fileName, List<T> data)`

```csharp
public List<T> Load<T>(string fileName)
{
    string path = Path.Combine(BasePath, fileName);  // BasePath = "Data"
    if (!File.Exists(path))
        return new List<T>();
    string json = File.ReadAllText(path);
    return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
}

public void Save<T>(string fileName, List<T> data)
{
    string path = Path.Combine(BasePath, fileName);
    string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(path, json);
}
```

**Files involved in a return operation:**

| File | Entity | What changes |
|---|---|---|
| `Data/loans.json` | `Loan` | One loan's `ReturnDate` is set to current timestamp |
| `Data/books.json` | `Book` | One book's `CopiesAvailable` is incremented by 1 |

---

## 6. Display Logic — How the Grid Shows Return Status

In `LoadLoans()` (MyLoans.cs:69), the `LoanDisplay` record projects `ReturnDate` as a **string**:

```csharp
ReturnDate = l.ReturnDate.HasValue
    ? l.ReturnDate.Value.ToString()
    : "کتاب هنوز در دست مشتری است",
```

- If `ReturnDate` has a value → shows the date/time string (e.g. `"2026/06/30 10:45:00"`)
- If `ReturnDate` is null (not returned) → shows `"کتاب هنوز در دست مشتری است"`

The `LoanDisplay` record:

```csharp
private record LoanDisplay
{
    public int Id { get; init; }          // Hidden column
    public string BookName { get; init; } // Displayed as "نام کتاب"
    public DateTime LoanDate { get; init; }    // Displayed as "تاریخ امانت"
    public DateTime DueDate { get; init; }     // Displayed as "تاریخ بازگشت"
    public string ReturnDate { get; init; }    // Displayed as "تاریخ برگردانده شده"
}
```

Column headers are set to Persian after data binding:
```csharp
dgvLoans.Columns["BookName"].HeaderText = "نام کتاب";
dgvLoans.Columns["LoanDate"].HeaderText = "تاریخ امانت";
dgvLoans.Columns["DueDate"].HeaderText = "تاریخ بازگشت";
dgvLoans.Columns["ReturnDate"].HeaderText = "تاریخ برگردانده شده";
dgvLoans.Columns["Id"]!.Visible = false;  // Hidden
```

---

## 7. Summary — Complete Call Chain

```
User clicks "برگرداندن کتاب"
    │
    ▼
MyLoans.BtnReturn_Click()
    │
    ├─ _loanService.GetLoanById(loanId)
    │      └─ JsonLoanRepository.GetById(id)
    │             └─ JsonRepository<T>.GetById(id)
    │                    └─ JsonDataStore.Load<Loan>("loans.json")
    │                           └─ File.ReadAllText → JsonSerializer.Deserialize
    │
    ├─ _bookService.GetBookById(loan.BookId)
    │      └─ JsonBookRepository.GetById(id)
    │             └─ JsonRepository<T>.GetById(id)
    │                    └─ JsonDataStore.Load<Book>("books.json")
    │
    ├─ _loanService.ReturnBook(loanId, book)      ← sets ReturnDate, increments CopiesAvailable
    │      └─ JsonLoanRepository.Update(loan)
    │             └─ JsonRepository<T>.Update(loan)
    │                    └─ JsonDataStore.Save<Loan>("loans.json")
    │
    ├─ _bookService.UpdateBook(book)               ← persists the incremented CopiesAvailable
    │      └─ JsonBookRepository.Update(book)
    │             └─ JsonRepository<T>.Update(book)
    │                    └─ JsonDataStore.Save<Book>("books.json")
    │
    ├─ LoadLoans()                                  ← re-binds the grid
    └─ MessageBox.Show("کتاب با موفقیت برگردانده شد")
```

---

## 8. Known Gaps / Future Improvements

| Issue | Description |
|---|---|
| **No idempotency** | Calling `ReturnBook` twice on the same loan overwrites `ReturnDate` and increments `CopiesAvailable` again — corrupting data |
| **No late-fine calculation** | When `ReturnDate > DueDate`, no `Debt` is created. The `DebtService` exists but is never called from the return flow |
| **`BookReturned` event unused** | The event is fired but nothing subscribes to it. Could be used to auto-create notifications or debts |
| **Book save not automatic** | `LoanService.ReturnBook` modifies `book.CopiesAvailable` in memory but doesn't persist it — the caller must remember to call `bookService.UpdateBook()` |
