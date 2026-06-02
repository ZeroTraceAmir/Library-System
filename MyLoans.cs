using System.Windows.Forms;

namespace library_system
{
    public class MyLoans : Form
    {
        public MyLoans()
        {
            this.Text = "دیدن کتاب هایی که قرض گرفتید";
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
