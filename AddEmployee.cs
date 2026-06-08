using System.Drawing;
using System.Windows.Forms;
using library_system.Services;

namespace library_system
{
    public class AddEmployee : Form
    {
        private readonly UserService _userService;

        public AddEmployee()
        {
            var store = new Repositories.JsonDataStore();
            _userService = new UserService(new Repositories.JsonUserRepository(store));

            this.Text = "اضافه کردن کارمند به کتاب خانه";
            this.WindowState = FormWindowState.Maximized;

            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(80, 40, 80, 0),
                AutoSize = true,
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

            string[] labels = { "نام:", "شماره تماس:", "رمز عبور:", "تکرار رمز عبور:", "نقش:" };
            var txtName = new TextBox();
            var txtPhone = new TextBox();
            var txtPassword = new TextBox { PasswordChar = '*' };
            var txtRepeatPassword = new TextBox { PasswordChar = '*' };
            var cmbRole = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "ادمین", "کارمند" },
                SelectedIndex = 0,
            };

            Control[] inputs = { txtName, txtPhone, txtPassword, txtRepeatPassword, cmbRole };

            for (int i = 0; i < 5; i++)
            {
                table.Controls.Add(new Label
                {
                    Text = labels[i],
                    Font = new Font("Tahoma", 11F),
                    TextAlign = ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill,
                }, 0, i);

                inputs[i].Font = new Font("Tahoma", 11F);
                inputs[i].Dock = DockStyle.Fill;
                inputs[i].Padding = new Padding(5);
                table.Controls.Add(inputs[i], 1, i);
            }

            var btnAdd = new Button
            {
                Text = "اضافه کردن",
                Font = new Font("Tahoma", 11F),
                Height = 40,
                AutoSize = true,
            };
            btnAdd.Click += (s, e) =>
            {
                try
                {
                    var name = txtName.Text.Trim();
                    var phone = txtPhone.Text.Trim();
                    var password = txtPassword.Text.Trim();
                    var repeatPassword = txtRepeatPassword.Text.Trim();
                    var role = cmbRole.SelectedIndex;

                    _userService.AddEmployee(name, phone, password, repeatPassword, role);

                    MessageBox.Show(this, "کارمند با موفقیت اضافه شد", "موفق",
                        MessageBoxButtons.OK, MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "خطا",
                        MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                }
            };
            table.Controls.Add(btnAdd, 1, 5);

            Controls.Add(table);

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
