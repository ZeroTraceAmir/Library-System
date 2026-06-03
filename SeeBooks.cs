using System.Windows.Forms;

namespace library_system
{
    public class SeeBooks : Form
    {
        public SeeBooks()
        {
            this.Text = "دیدن کتاب های";
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
