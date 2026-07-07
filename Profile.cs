using System.Drawing;
using System.Windows.Forms;
using library_system.Services;

namespace library_system
{
    public class Profile : Form
    {
        private readonly string _name;
        private readonly string _phone;
        private readonly string _role;
        private readonly bool _isUser;
        private readonly UserService _userService;
        private readonly CustomerService _customerService;

        public Profile(string name, string phone, string role, bool isUser)
        {
            _name = name;
            _phone = phone;
            _role = role;
            _isUser = isUser;

            Repositories.JsonDataStore store = new Repositories.JsonDataStore();
            _userService = new UserService(new Repositories.JsonUserRepository(store));
            _customerService = new CustomerService(new Repositories.JsonCustomerRepository(store));

            this.Text = "پروفایل";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(17, 21, 32);

            BuildHeader(name, phone, role);
            BuildLogoutButton();
            BuildFields();
            BuildBackButton();
        }

        private void BuildHeader(string name, string phone, string role)
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(17, 21, 32),
            };

            FlowLayoutPanel flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(15),
            };

            void AddLabel(string text, float fontSize, bool bold, Color color)
            {
                flow.Controls.Add(
                    new Label
                    {
                        Text = text,
                        Font = new Font(
                            "Tahoma",
                            fontSize,
                            bold ? FontStyle.Bold : FontStyle.Regular
                        ),
                        ForeColor = color,
                        AutoSize = true,
                        Margin = new Padding(20, 15, 0, 0),
                    }
                );
            }

            AddLabel(role, 11F, true, Color.FromArgb(46, 204, 113));
            AddLabel(phone, 10F, false, Color.White);
            AddLabel(name, 11F, true, Color.White);

            panel.Controls.Add(flow);
            Controls.Add(panel);
        }

        private void BuildLogoutButton()
        {
            Button btnLogout = new Button
            {
                Text = "خروج",
                Font = new Font("Tahoma", 11F),
                AutoSize = true,
                Height = 40,
                Margin = new Padding(15),
                BackColor = Color.FromArgb(0, 255, 156),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) =>
            {
                if (_isUser)
                    _userService.Logout(_phone);
                else
                    _customerService.Logout(_phone);

                foreach (Form f in Application.OpenForms)
                {
                    if (f is Form1)
                    {
                        f.Show();
                        break;
                    }
                }

                Owner?.Close();
                this.Close();
            };
            Controls.Add(btnLogout);
        }

        private void BuildFields()
        {
            TableLayoutPanel table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(80, 40, 80, 0),
                AutoSize = true,
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

            string[] labels = { "نام:", "شماره تماس:", "رمز عبور:" };
            string[] values = { _name, _phone, "" };

            TextBox txtName = new TextBox();
            TextBox txtPhone = new TextBox();
            TextBox txtPassword = new TextBox();
            txtPassword.PasswordChar = '*';

            TextBox[] inputs = { txtName, txtPhone, txtPassword };

            for (int i = 0; i < 3; i++)
            {
                table.Controls.Add(
                    new Label
                    {
                        Text = labels[i],
                        Font = new Font("Tahoma", 11F),
                        TextAlign = ContentAlignment.MiddleRight,
                        Dock = DockStyle.Fill,
                    },
                    0,
                    i
                );

                inputs[i].Text = values[i];
                inputs[i].Font = new Font("Tahoma", 11F);
                inputs[i].Dock = DockStyle.Fill;
                inputs[i].Padding = new Padding(5);
                inputs[i].BackColor = Color.FromArgb(37, 40, 54);
                inputs[i].ForeColor = Color.White;
                inputs[i].BorderStyle = BorderStyle.FixedSingle;
                table.Controls.Add(inputs[i], 1, i);
            }

            Button btnSave = new Button
            {
                Text = "ذخیره",
                Font = new Font("Tahoma", 11F),
                Height = 40,
                AutoSize = true,
                BackColor = Color.FromArgb(0, 255, 156),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) =>
            {
                string name = txtName.Text.Trim();
                string phone = txtPhone.Text.Trim();
                string password = txtPassword.Text.Trim();

                bool hasName = !string.IsNullOrEmpty(name);
                bool hasPhone = !string.IsNullOrEmpty(phone);
                bool hasPassword = !string.IsNullOrEmpty(password);

                if (!hasName && !hasPhone)
                {
                    MessageBox.Show(
                        "حداقل یکی از فیلدهای نام یا شماره تماس باید پر شود",
                        "خطا",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                if (!hasPassword)
                {
                    MessageBox.Show(
                        "رمز عبور نمی‌تواند خالی باشد",
                        "خطا",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                if (_isUser)
                    _userService.UserProfileEdit(name, phone, password, _phone);
                else
                    _customerService.CustomerProfileEdit(name, phone, password, _phone);
                MessageBox.Show(
                    "عملیات با موفقیت انجام شد. تغیرات بعد از ورود مجدد به سامانه قابل مشاهده خواهد بود",
                    "موفق",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            };
            table.Controls.Add(btnSave, 1, 3);

            Controls.Add(table);

            if (!_isUser)
            {
                Button btnDelete = new Button
                {
                    Text = "حذف حساب",
                    Font = new Font("Tahoma", 11F),
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(231, 76, 60),
                    FlatStyle = FlatStyle.Flat,
                    AutoSize = true,
                    Height = 40,
                };
                btnDelete.FlatAppearance.BorderSize = 0;
                btnDelete.Click += (s, e) =>
                {
                    _customerService.DeleteCustomerAcc(_phone);
                    Application.Exit();
                };
                btnDelete.Dock = DockStyle.Top;
                Controls.Add(btnDelete);
            }
        }

        private void BuildBackButton()
        {
            Button btnBack = new Button
            {
                Text = "بازگشت",
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(0, 255, 156),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += (s, e) => this.Close();
            Controls.Add(btnBack);
        }
    }
}
