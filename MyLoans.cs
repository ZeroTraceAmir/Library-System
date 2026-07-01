using System.Linq;
using System.Windows.Forms;
using library_system.Models;
using library_system.Repositories;
using library_system.Services;

namespace library_system
{
    public class MyLoans : Form
    {
        private DataGridView dgvLoans;
        private Button btnReturn;
        private Button btnBack;
        private readonly CustomerService _customerService;
        private readonly LoanService _loanService;
        private readonly BookService _bookService;

        public MyLoans()
        {
            JsonDataStore store = new JsonDataStore();
            _customerService = new CustomerService(new JsonCustomerRepository(store));
            _loanService = new LoanService(new JsonLoanRepository(store));
            _bookService = new BookService(new JsonBookRepository(store));

            InitializeComponent();
            LoadLoans();
        }

        private void InitializeComponent()
        {
            this.Text = "امانت های من";
            this.WindowState = FormWindowState.Maximized;

            dgvLoans = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = ColorTranslator.FromHtml("#111520"),
            };

            btnReturn = new Button
            {
                Text = "برگرداندن کتاب",
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = ColorTranslator.FromHtml("#00ff9c"),
                ForeColor = ColorTranslator.FromHtml("#111520"),
                Font = new Font("Vazir", 11F, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 },
            };

            btnReturn.Click += BtnReturn_Click;

            btnBack = new Button
            {
                Text = "بازگشت",
                Dock = DockStyle.Bottom,
                Height = 50,
                DialogResult = DialogResult.Cancel,
                BackColor = ColorTranslator.FromHtml("#00ff9c"),
                ForeColor = ColorTranslator.FromHtml("#111520"),
                Font = new Font("Vazir", 11F, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 },
            };

            // Set the background color for all rows
            dgvLoans.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#1f293d"); // Choose your color
            dgvLoans.DefaultCellStyle.ForeColor = Color.White; // Text color
            dgvLoans.DefaultCellStyle.Font = new Font("Vazir", 9F);
            dgvLoans.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#00ff9c"); // Highlight color when clicked
            dgvLoans.DefaultCellStyle.SelectionForeColor = ColorTranslator.FromHtml("#111520");

            // Enable headers visual styles if you haven't already (important for header colors to show up)
            dgvLoans.EnableHeadersVisualStyles = false;
            dgvLoans.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#00ff9c");
            dgvLoans.ColumnHeadersDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#111520");
            dgvLoans.ColumnHeadersDefaultCellStyle.Font = new Font("Vazir", 9F, FontStyle.Bold);
            dgvLoans.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            Controls.Add(dgvLoans);
            Controls.Add(btnReturn);
            Controls.Add(btnBack);
        }

        private void LoadLoans()
        {
            Customer? customer = _customerService.GetLoggedInCustomer();

            if (customer == null)
                return;

            List<Loan> loans = _loanService.GetLoansByCustomerId(customer.Id);
            Dictionary<int, Book> books = _bookService.GetAllBooks().ToDictionary(b => b.Id);

            List<LoanDisplay> loanData = loans
                .Select(l => new LoanDisplay
                {
                    Id = l.Id,
                    BookName = books.TryGetValue(l.BookId, out Book book) ? book.Title : "نامشخص",
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate.HasValue ? l.ReturnDate.Value.ToString() : "کتاب هنوز در دست مشتری است",
                })
                .ToList();

            dgvLoans.DataSource = loanData;

            dgvLoans.Columns["Id"]!.Visible = false;
            dgvLoans.Columns["BookName"].HeaderText = "نام کتاب";
            dgvLoans.Columns["LoanDate"].HeaderText = "تاریخ امانت";
            dgvLoans.Columns["DueDate"].HeaderText = "تاریخ بازگشت";
            dgvLoans.Columns["ReturnDate"].HeaderText = "تاریخ برگردانده شده";
        }

        private void BtnReturn_Click(object? sender, EventArgs e)
        {
            if (dgvLoans.CurrentRow == null)
            {
                MessageBox.Show("ابتدا یک امانت را انتخاب کنید");
                return;
            }

            int loanId = Convert.ToInt32(dgvLoans.CurrentRow.Cells["Id"].Value);

            Loan? loan = _loanService.GetLoanById(loanId);

            if (loan == null)
                return;

            Book? book = _bookService.GetBookById(loan.BookId);

            if (book == null)
                return;

            _loanService.ReturnBook(loanId, book);

            _bookService.UpdateBook(book);

            LoadLoans();

            MessageBox.Show("کتاب با موفقیت برگردانده شد");
        }

        private record LoanDisplay
        {
            public int Id { get; init; }
            public string BookName { get; init; }
            public DateTime LoanDate { get; init; }
            public DateTime DueDate { get; init; }
            public string ReturnDate { get; init; }
        }
    }
}
