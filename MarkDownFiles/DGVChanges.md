# DGV Changes — MyLoans.cs DataGridView

## What changed in `MyLoans.cs`

### 1. Hidden columns: `Id` and `CustomerId`
- `Id` column is still present in the data source but set to `Visible = false` so it can still be read by `BtnReturn_Click` (`Cells["Id"].Value`).
- `CustomerId` is simply excluded from the anonymous projection — it is never added to the grid.

### 2. `BookId` replaced with book title
Instead of binding `List<Loan>` directly, `LoadLoans()` now:
1. Fetches all loans for the logged-in customer
2. Loads **all books** into a `Dictionary<int, Book>` (one query, O(1) lookups — avoids N+1)
3. Projects each loan into an **anonymous object** with:
   - `Id` (hidden) — kept for the return-button logic
   - `نام_کتاب` — the book's `Title` looked up from the dictionary (falls back to `"نامشخص"`)
   - `تاریخ_امانت` — `LoanDate`
   - `تاریخ_بازگشت` — `DueDate`
   - `تاریخ_برگردانده_شده` — `ReturnDate`

### 3. Column headers are in Persian
Headers are set explicitly after binding via `Column["..."].HeaderText`.

### Architecture flow
```
UI (MyLoans)  →  Service  →  Repository  →  Data/ (JSON files)
```
