using System.Windows.Forms;
using library_system.Repositories;
using library_system.Services;
using library_system.Models;
using System;

namespace library_system
{
    public class PayDebt : Form
    {
        private DataGridView dgvDebts;
        private Button btnPay;
        private Button btnBack;

        public PayDebt()
        {
            InitializeComponent();
            LoadDebts();
            btnPay.Click += BtnPay_Click;
        }

        private void InitializeComponent()
        {
            this.Text = "پرداخت بدهی";
            this.WindowState = FormWindowState.Maximized;

            dgvDebts = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            btnPay = new Button
            {
                Text = "پرداخت بدهی",
                Dock = DockStyle.Bottom,
                Height = 50
            };

            btnBack = new Button
            {
                Text = "بازگشت",
                Dock = DockStyle.Bottom,
                Height = 50,
                DialogResult = DialogResult.Cancel
            };

            Controls.Add(dgvDebts);
            Controls.Add(btnPay);
            Controls.Add(btnBack);
        }

        private void LoadDebts()
        {
            JsonDataStore store = new JsonDataStore();

            JsonCustomerRepository customerRepository = new JsonCustomerRepository(store);
            CustomerService customerService = new CustomerService(customerRepository);

            Customer? customer = customerService.GetLoggedInCustomer();

            if (customer == null)
                return;

            JsonDebtRepository debtRepository = new JsonDebtRepository(store);
            DebtService debtService = new DebtService(debtRepository);

            dgvDebts.DataSource =
                debtService.GetCustomerDebts(customer.Id);
        }

        private void BtnPay_Click(object? sender, EventArgs e)
        {
            if (dgvDebts.CurrentRow == null)
            {
                MessageBox.Show("ابتدا یک بدهی را انتخاب کنید");
                return;
            }

            int debtId =
                Convert.ToInt32(
                    dgvDebts.CurrentRow.Cells["Id"].Value);

            JsonDataStore store = new JsonDataStore();

            JsonDebtRepository debtRepository =
                new JsonDebtRepository(store);

            DebtService debtService =
                new DebtService(debtRepository);

            debtService.PayDebt(debtId);

            LoadDebts();

            MessageBox.Show("بدهی با موفقیت پرداخت شد");
        }
    }

}
