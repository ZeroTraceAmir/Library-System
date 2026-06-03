using System.Windows.Forms;

namespace library_system
{
    public class AddBook : Form
    {
        public AddBook()
        {
            this.Text = "اضافه کردن کتاب";
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
