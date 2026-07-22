# Passing Data from MyLoans to ReturnBook

You have several options in Windows Forms. Here they are from simplest to most flexible:

## Option 1: Constructor Parameter

Add a parameter to `ReturnBook`'s constructor, store it in a field.

**ReturnBook.cs**
```csharp
public class ReturnBook : Form
{
    private readonly Loan _loan;

    public ReturnBook(Loan loan)  // ← parameter here
    {
        _loan = loan;
        InitializeComponent();
        // now use _loan anywhere in the form
    }
}
```

**MyLoans.cs** (when opening the form)
```csharp
Loan selectedLoan = ...; // get it from the DataGridView
ReturnBook form = new ReturnBook(selectedLoan);
form.ShowDialog();
```

## Option 2: Public Property

Add a public property on `ReturnBook` and set it before `ShowDialog`.

**ReturnBook.cs**
```csharp
public class ReturnBook : Form
{
    public Loan SelectedLoan { get; set; }
    ...
}
```

**MyLoans.cs**
```csharp
if (dgvLoans.CurrentRow == null)
{
    MessageBox.Show("ابتدا یک امانت را انتخاب کنید");
    return;
}

int loanId = Convert.ToInt32(dgvLoans.CurrentRow.Cells["Id"].Value);

Loan? selectedLoan = _loanService.GetLoanById(loanId);

if (selectedLoan == null)
    return;

ReturnBook form = new ReturnBook(selectedLoan);
form.ShowDialog();
```

## Option 2: Public Property

Add a public property on `ReturnBook` and set it before `ShowDialog`.

**ReturnBook.cs**
```csharp
public class ReturnBook : Form
{
    public Loan SelectedLoan { get; set; }
    ...
}
```

**MyLoans.cs**
```csharp
if (dgvLoans.CurrentRow == null)
{
    MessageBox.Show("ابتدا یک امانت را انتخاب کنید");
    return;
}

int loanId = Convert.ToInt32(dgvLoans.CurrentRow.Cells["Id"].Value);

Loan? selectedLoan = _loanService.GetLoanById(loanId);

if (selectedLoan == null)
    return;

ReturnBook form = new ReturnBook();
form.SelectedLoan = selectedLoan;
form.ShowDialog();
```

## Option 3: Public Method

Create a load method that accepts the data.

**ReturnBook.cs**
```csharp
public void LoadLoan(Loan loan)
{
    // populate controls with loan data
}
```

**MyLoans.cs**
```csharp
if (dgvLoans.CurrentRow == null)
{
    MessageBox.Show("ابتدا یک امانت را انتخاب کنید");
    return;
}

int loanId = Convert.ToInt32(dgvLoans.CurrentRow.Cells["Id"].Value);

Loan? selectedLoan = _loanService.GetLoanById(loanId);

if (selectedLoan == null)
    return;

ReturnBook form = new ReturnBook();
form.LoadLoan(selectedLoan);
form.ShowDialog();
```

## Which one to use?

**Option 1 (constructor)** is the most common pattern in this codebase — look at how services are injected in existing forms. It guarantees the data is available from the moment the form exists.

**Option 2 (property)** is useful if the data might not always be required, or if you want to set it after construction.

Both are fine. Pick whichever feels more natural to you.

---

**Note:** You'll need to add `using library_system.Models;` in `ReturnBook.cs` to access the `Loan` type.
