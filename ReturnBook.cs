using System.Windows.Forms;
using library_system.Models;
using library_system.Services;

namespace library_system
{
    public class ReturnBook : Form
    {
        private readonly Loan _loan;
        private readonly Book? _book;
        private ComboBox cmbSituation;

        //private readonly DebtService debtService;
        private readonly LoanService _loanService;
        private readonly DebtService _debtService;
        //private readonly CustomerService _customerService;
        private readonly Repositories.JsonCustomerRepository _customerRepository;

        public ReturnBook(Loan loan, Book? book)
        {
            _loan = loan;
            _book = book;

            Repositories.JsonDataStore store = new Repositories.JsonDataStore();
            _customerRepository = new Repositories.JsonCustomerRepository(store);
            _loanService = new LoanService(
                new Repositories.JsonLoanRepository(store),
                new Repositories.JsonBookRepository(store),
                new Repositories.JsonDebtRepository(store),
                _customerRepository);
            _debtService = new DebtService(new Repositories.JsonDebtRepository(store));
            //_customerService = new CustomerService(_customerRepository);

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "باز گرداندن کتاب";
            this.ClientSize = new Size(520, 470);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = ColorTranslator.FromHtml("#111520");
            this.ForeColor = Color.White;
            this.Font = new Font("Vazir", 11F);

            string bookName = _book?.Title ?? "نامشخص";
            string author = _book?.Author ?? "نامشخص";
            string loanDate = _loan.LoanDate.ToString("yyyy/MM/dd");
            string dueDate = _loan.DueDate.ToString("yyyy/MM/dd");

            // ===== Title =====
            Label lblTitle = new Label
            {
                Text = "باز گرداندن کتاب",
                Font = new Font("Vazir", 20F, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#00ff9c"),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(520, 55),
                Location = new Point(0, 20),
            };

            // ===== Card =====
            Panel card = new Panel
            {
                Size = new Size(440, 180),
                Location = new Point(40, 90),
                BackColor = ColorTranslator.FromHtml("#1A1F2E"),
                BorderStyle = BorderStyle.FixedSingle,
            };

            Label lblInfo = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Vazir", 12F),
                ForeColor = Color.White,
                Padding = new Padding(20),
                TextAlign = ContentAlignment.TopRight,
                Text =
                    $"کتاب: {bookName}\n\n"
                    + $"نویسنده: {author}\n\n"
                    + $"تاریخ امانت: {loanDate}\n\n"
                    + $"تاریخ بازگشت: {dueDate}",
            };

            card.Controls.Add(lblInfo);

            // ===== Situation =====
            Label lblSituation = new Label
            {
                Text = "وضعیت کتاب",
                Font = new Font("Vazir", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(365, 290),
            };

            cmbSituation = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Vazir", 11F),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(220, 38),
                Location = new Point(200, 325),
                BackColor = ColorTranslator.FromHtml("#252836"),
                ForeColor = Color.White,
            };

            cmbSituation.Items.AddRange(new object[] { "سالم", "آسیب دیده", "گم شده" });

            cmbSituation.SelectedIndex = 0;

            // ===== Buttons =====
            Button btnReturnBook = new Button
            {
                Text = "بازگرداندن کتاب",
                Size = new Size(170, 48),
                Location = new Point(110, 395),
                BackColor = ColorTranslator.FromHtml("#00ff9c"),
                ForeColor = ColorTranslator.FromHtml("#111520"),
                Font = new Font("Vazir", 11F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
            };
            btnReturnBook.FlatAppearance.BorderSize = 0;
            btnReturnBook.Click += BtnReturn_Click;
            Button btnCancel = new Button
            {
                Text = "لغو",
                Size = new Size(120, 48),
                Location = new Point(295, 395),
                BackColor = ColorTranslator.FromHtml("#2D3448"),
                ForeColor = Color.White,
                Font = new Font("Vazir", 11F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel,
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            // ===== Hover Effects =====
            btnReturnBook.MouseEnter += (s, e) =>
                btnReturnBook.BackColor = ColorTranslator.FromHtml("#00d985");

            btnReturnBook.MouseLeave += (s, e) =>
                btnReturnBook.BackColor = ColorTranslator.FromHtml("#00ff9c");

            btnCancel.MouseEnter += (s, e) =>
                btnCancel.BackColor = ColorTranslator.FromHtml("#3A425B");

            btnCancel.MouseLeave += (s, e) =>
                btnCancel.BackColor = ColorTranslator.FromHtml("#2D3448");

            // ===== Add Controls =====
            Controls.Add(lblTitle);
            Controls.Add(card);
            Controls.Add(lblSituation);
            Controls.Add(cmbSituation);
            Controls.Add(btnReturnBook);
            Controls.Add(btnCancel);

            AcceptButton = btnReturnBook;
            CancelButton = btnCancel;
        }

        private void BtnReturn_Click(object? sender, EventArgs e)
        {
            //int index = cmbSituation.SelectedIndex;
            //if (index == 0) { }

            try
            {
                if (_book == null)
                {
                    MessageBox.Show("اطلاعات کتاب یافت نشد");
                    return;
                }

                _loanService.ReturnBook(_loan.Id, _book);

                int index = cmbSituation.SelectedIndex;

                if (index == 1)
                {
                    decimal amount = (decimal)(_book.LostChargePrice / 2);
                    _debtService.AddDebt(new Debt
                    {
                        CustomerId = _loan.CustomerId,
                        Amount = amount,
                        Reason = "کتاب اسیب دیده",
                        IsPaid = false,
                    });

                    Customer? customer = _customerRepository.GetById(_loan.CustomerId);
                    if (customer != null)
                    {
                        customer.Debt += amount;
                        _customerRepository.Update(customer);
                    }
                }
                else if (index == 2)
                {
                    decimal amount = (decimal)_book.LostChargePrice;
                    _debtService.AddDebt(new Debt
                    {
                        CustomerId = _loan.CustomerId,
                        Amount = amount,
                        Reason = "کتاب گم شده",
                        IsPaid = false,
                    });

                    Customer? customer = _customerRepository.GetById(_loan.CustomerId);
                    if (customer != null)
                    {
                        customer.Debt += amount;
                        _customerRepository.Update(customer);
                    }
                }

                MessageBox.Show("کتاب با موفقیت بازگردانده شد");
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
