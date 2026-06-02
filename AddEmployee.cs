using System.Windows.Forms;

namespace library_system
{
    public class AddEmployee : Form
    {
        public AddEmployee()
        {
            this.Text = "اضافه کردن کارمند به کتاب خانه";
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
