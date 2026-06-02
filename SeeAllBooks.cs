using System.Windows.Forms;

namespace library_system
{
    public class SeeAllBooks : Form
    {
        public SeeAllBooks()
        {
            this.Text = "دیدن همه کتاب ها";
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
