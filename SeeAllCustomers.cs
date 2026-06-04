using System.Windows.Forms;

namespace library_system
{
    public class SeeAllCustomers : Form
    {
        public SeeAllCustomers()
        {
            this.Text = "دیدن همه مشتریان کتابخانه";
            this.WindowState = FormWindowState.Maximized;
            Controls.Add(
                new Button
                {
                    Text = "بازگشت",
                    Dock = DockStyle.Bottom,
                    Height = 50,
                    DialogResult = DialogResult.Cancel,
                }
            );
        }
    }
}
