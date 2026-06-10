using System.Windows.Forms;
using library_system.Repositories;
using library_system.Services;
using library_system.Models;

namespace library_system
{
    public class MyLoans : Form
    {
        private DataGridView dgvLoans;
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

            btnBack = new Button
            {
                Text = "بازگشت",
                Dock = DockStyle.Bottom,
                Height = 50,
                DialogResult = DialogResult.Cancel
            };

            Controls.Add(dgvLoans);
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
    }

}
