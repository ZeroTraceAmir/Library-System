using System;
using System.Linq;
using System.Windows.Forms;
using library_system.Models;
using library_system.Repositories;
using library_system.Services;

namespace library_system
{
    public class PayDebt : Form
    {
        private DataGridView dgvDebts;
        private Button btnPay;
        private Button btnBack;
        private readonly CustomerService _customerService;
        private readonly DebtService _debtService;
        private readonly BookService _bookService;

        public PayDebt()
        {
            JsonDataStore store = new JsonDataStore();
            _customerService = new CustomerService(new JsonCustomerRepository(store));
            _debtService = new DebtService(new JsonDebtRepository(store));
            _bookService = new BookService(new JsonBookRepository(store));

            InitializeComponent();
            LoadDebts();
        }

        private void InitializeComponent()
        {
            this.Text = "بدهی‌های من";
            this.WindowState = FormWindowState.Maximized;

            dgvDebts = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = ColorTranslator.FromHtml("#111520"),
            };

            btnPay = new Button
            {
                Text = "پرداخت بدهی",
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = ColorTranslator.FromHtml("#00ff9c"),
                ForeColor = ColorTranslator.FromHtml("#111520"),
                Font = new Font("Vazir", 11F, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 },
            };

            btnPay.Click += BtnPay_Click;

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

            dgvDebts.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#1f293d");
            dgvDebts.DefaultCellStyle.ForeColor = Color.White;
            dgvDebts.DefaultCellStyle.Font = new Font("Vazir", 9F);
            dgvDebts.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#00ff9c");
            dgvDebts.DefaultCellStyle.SelectionForeColor = ColorTranslator.FromHtml("#111520");

            dgvDebts.EnableHeadersVisualStyles = false;
            dgvDebts.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#00ff9c");
            dgvDebts.ColumnHeadersDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#111520");
            dgvDebts.ColumnHeadersDefaultCellStyle.Font = new Font("Vazir", 9F, FontStyle.Bold);
            dgvDebts.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Controls.Add(dgvDebts);
            Controls.Add(btnPay);
            Controls.Add(btnBack);
        }

        private void LoadDebts()
        {
            Customer? customer = _customerService.GetLoggedInCustomer();

            if (customer == null)
                return;

            var books = _bookService.GetAllBooks().ToDictionary(b => b.Id);

            var debtData = _debtService
                .GetCustomerDebts(customer.Id)
                .Select(d => new DebtDisplay
                {
                    Id = d.Id,
                    Amount = d.Amount,
                    Reason = d.Reason,
                    IsPaid = d.IsPaid ? "پرداخت شده" : "پرداخت نشده",
                })
                .ToList();

            dgvDebts.DataSource = debtData;

            dgvDebts.Columns["Id"]!.Visible = false;
            dgvDebts.Columns["Amount"].HeaderText = "مبلغ";
            dgvDebts.Columns["Reason"].HeaderText = "دلیل";
            dgvDebts.Columns["IsPaid"].HeaderText = "وضعیت";
        }

        private void BtnPay_Click(object? sender, EventArgs e)
        {
            if (dgvDebts.CurrentRow == null)
            {
                MessageBox.Show("ابتدا یک بدهی را انتخاب کنید");
                return;
            }

            int debtId = Convert.ToInt32(dgvDebts.CurrentRow.Cells["Id"].Value);

            try
            {
                _debtService.PayDebt(debtId);

                LoadDebts();

                MessageBox.Show("بدهی با موفقیت پرداخت شد");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private record DebtDisplay
        {
            public int Id { get; init; }
            public decimal Amount { get; init; }
            public string Reason { get; init; }
            public string IsPaid { get; init; }
        }
    }
}
