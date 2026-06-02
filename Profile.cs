using System.Windows.Forms;

namespace library_system
{
    public class Profile : Form
    {
        public Profile(string name, string phone, string role)
        {
            this.Text = "پروفایل";
            this.WindowState = FormWindowState.Maximized;
        }
    }
}
