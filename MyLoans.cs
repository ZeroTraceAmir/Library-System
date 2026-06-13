using System.Windows.Forms;
using library_system.Repositories;
using library_system.Services;
using library_system.Models;

namespace library_system
{
    public class MyLoans : Form
    {
        private DataGridView dgvLoans;
        private Button btnReturn;
        private Button btnBack;

        public MyLoans()
        {
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
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            btnReturn = new Button
            {
                Text = "برگرداندن کتاب",
                Dock = DockStyle.Bottom,
                Height = 50
            };

            btnReturn.Click += BtnReturn_Click;

            btnBack = new Button
            {
                Text = "بازگشت",
                Dock = DockStyle.Bottom,
                Height = 50,
                DialogResult = DialogResult.Cancel
            };

            Controls.Add(dgvLoans);
            Controls.Add(btnReturn);
            Controls.Add(btnBack);
        }

        private void LoadLoans()
        {
            JsonDataStore store = new JsonDataStore();

            var customerRepository = new JsonCustomerRepository(store);
            var customerService = new CustomerService(customerRepository);

            Customer? customer = customerService.GetLoggedInCustomer();

            if (customer == null)
                return;

            var loanRepository = new JsonLoanRepository(store);
            var loanService = new LoanService(loanRepository);

            dgvLoans.DataSource =
                loanService.GetLoansByCustomerId(customer.Id);
        }

        private void BtnReturn_Click(object? sender, EventArgs e)
        {
            if (dgvLoans.CurrentRow == null)
            {
                MessageBox.Show("ابتدا یک امانت را انتخاب کنید");
                return;
            }

            int loanId =
                Convert.ToInt32(
                    dgvLoans.CurrentRow.Cells["Id"].Value);

            JsonDataStore store = new JsonDataStore();

            var loanRepository =
                new JsonLoanRepository(store);

            var loanService =
                new LoanService(loanRepository);

            Loan? loan =
                loanService.GetLoanById(loanId);

            if (loan == null)
                return;

            var bookRepository =
                new JsonBookRepository(store);

            Book? book =
                bookRepository.GetById(loan.BookId);

            if (book == null)
                return;

            loanService.ReturnBook(loanId, book);

            bookRepository.Update(book);

            LoadLoans();

            MessageBox.Show("کتاب با موفقیت برگردانده شد");

        }
    }

}
