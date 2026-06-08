using System.Windows.Forms;

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
        }

        private void InitializeComponent()
        {
            this.Text = "پرداخت بدهی";
            this.WindowState = FormWindowState.Maximized;

            dgvDebts = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
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
    }

}
