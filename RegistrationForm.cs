using System.Drawing;
using System.Windows.Forms;
using library_system.Models;
using library_system.Services;

namespace library_system
{
    public class RegisterForm : Form
    {
        private readonly CustomerService customerService;

        private readonly TextBox txtName = new();
        private readonly TextBox txtNumber = new();
        private readonly TextBox txtPassword = new();
        private readonly TextBox txtConfirm = new();
        private readonly Button btnRegister = new();

        public RegisterForm(CustomerService customerService)
        {
            this.customerService = customerService;
            BuildUi();
        }

        private void BuildUi()
        {
            Text = "ثبت نام عضو جدید";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            Font = new Font("Tahoma", 10F);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(380, 320);
            BackColor = Color.White;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                ColumnCount = 2,
                RowCount = 5,
                RightToLeft = RightToLeft.Yes
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
            for (int i = 0; i < 5; i++)
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            layout.Controls.Add(MakeLabel("نام:"), 0, 0);
            layout.Controls.Add(Configure(txtName), 1, 0);

            layout.Controls.Add(MakeLabel("شماره تماس:"), 0, 1);
            layout.Controls.Add(Configure(txtNumber), 1, 1);

            layout.Controls.Add(MakeLabel("رمز عبور:"), 0, 2);
            txtPassword.UseSystemPasswordChar = true;
            layout.Controls.Add(Configure(txtPassword), 1, 2);

            layout.Controls.Add(MakeLabel("تکرار رمز عبور:"), 0, 3);
            txtConfirm.UseSystemPasswordChar = true;
            layout.Controls.Add(Configure(txtConfirm), 1, 3);

            btnRegister.Text = "ثبت نام";
            btnRegister.Dock = DockStyle.Fill;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.BackColor = Color.FromArgb(33, 150, 243);
            btnRegister.ForeColor = Color.White;
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Cursor = Cursors.Hand;
            btnRegister.Margin = new Padding(3, 10, 3, 3);
            btnRegister.Click += BtnRegister_Click;
            layout.Controls.Add(btnRegister, 0, 4);
            layout.SetColumnSpan(btnRegister, 2);

            Controls.Add(layout);
            AcceptButton = btnRegister;
        }

        private static Label MakeLabel(string text) => new()
        {
            Text = text,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight,
            RightToLeft = RightToLeft.Yes
        };

        private static TextBox Configure(TextBox box)
        {
            box.Dock = DockStyle.Fill;
            box.Margin = new Padding(3, 11, 3, 11);
            box.RightToLeft = RightToLeft.Yes;
            return box;
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            try
            {
                if (txtPassword.Text != txtConfirm.Text)
                    throw new Exception("رمز عبور و تکرار آن یکسان نیستند");

                if (txtPassword.Text.Length < 4)
                    throw new Exception("رمز عبور باید حداقل ۴ کاراکتر باشد");

                var customer = new Customer
                {
                    Name = txtName.Text.Trim(),
                    Number = txtNumber.Text.Trim(),
                    Password = txtPassword.Text,
                    IsLogedin = false
                };

                customerService.AddCustomer(customer);

                ShowMessage("ثبت نام با موفقیت انجام شد", "موفق", MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, "خطا", MessageBoxIcon.Error);
            }
        }

        private void ShowMessage(string text, string caption, MessageBoxIcon icon) =>
            MessageBox.Show(this, text, caption, MessageBoxButtons.OK, icon,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
    }
}
