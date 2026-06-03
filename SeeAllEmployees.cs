using System.Windows.Forms;

namespace library_system
{
    public class SeeAllEmployees : Form
    {
        public SeeAllEmployees()
        {
            this.Text = "دیدن همه کارمندان کتابخانه";
            this.WindowState = FormWindowState.Maximized;
            Controls.Add(new Button
            {
                Text = "بازگشت",
                Dock = DockStyle.Bottom,
                Height = 50,
                DialogResult = DialogResult.Cancel,
            });
        }
    }
}
