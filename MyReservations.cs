using System.Windows.Forms;

namespace library_system
{
    public class MyReservations : Form
    {
        private DataGridView dgvReservations;
        private Button btnBack;

        public MyReservations()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "رزروهای من";
            this.WindowState = FormWindowState.Maximized;

            dgvReservations = new DataGridView
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

            Controls.Add(dgvReservations);
            Controls.Add(btnBack);
        }
    }

}