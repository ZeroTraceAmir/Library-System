using System.Windows.Forms;

namespace library_system
{
    public class PayDebt : Form
    {
        public PayDebt()
        {
            this.Text = "پرداخت بدهی";
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
