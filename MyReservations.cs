using System.Windows.Forms;

namespace library_system
{
    public class MyReservations : Form
    {
        public MyReservations()
        {
            this.Text = "دیدن کتاب هایی که رزرو کردید";
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
