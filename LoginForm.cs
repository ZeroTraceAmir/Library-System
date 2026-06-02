// using System;
// using System.Drawing;
// using System.Windows.Forms;

// namespace library_system
// {
//     public class LoginForm : Form
//     {
//         public LoginForm()
//         {
//             Text = "ورود";
//             Size = new Size(450, 300);
//             StartPosition = FormStartPosition.CenterScreen;
//             RightToLeft = RightToLeft.Yes;
//             RightToLeftLayout = true;
//             BackColor = Color.White;

//             Label title = new Label
//             {
//                 Text = "ورود",
//                 Font = new Font("Tahoma", 18F, FontStyle.Bold),
//                 AutoSize = false,
//                 TextAlign = ContentAlignment.MiddleCenter,
//                 Dock = DockStyle.Top,
//                 Height = 70
//             };

//             Controls.Add(title);
//         }
//     }
// }

using System;
using System.Drawing;
using System.Windows.Forms;
using library_system.Services;

namespace library_system
{
    public class LoginForm : Form
    {
        private readonly CustomerService _customerService;

        // UI Controls
        private Label lblTitle;
        private Label lblPhoneNumber;
        private TextBox txtPhoneNumber;
        private Label lblPassword;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnBack;

        public LoginForm(CustomerService customerService)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form Configuration
            this.Text = "ورود به سیستم";
            this.Width = 400;
            this.Height = 350;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Tahoma", 9.5F, FontStyle.Regular);

            // RTL Support for Persian Layout
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;

            // Title
            lblTitle = new Label
            {
                Text = "ورود کاربران",
                Font = new Font("Tahoma", 14F, FontStyle.Bold),
                Location = new Point(20, 25),
                Size = new Size(345, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.DarkSlateGray
            };

            // Phone Number Label & Input
            lblPhoneNumber = new Label
            {
                Text = "شماره موبایل:",
                Location = new Point(30, 85),
                Size = new Size(100, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtPhoneNumber = new TextBox
            {
                Location = new Point(140, 85),
                Size = new Size(200, 25),
                MaxLength = 11
            };

            // Password Label & Input
            lblPassword = new Label
            {
                Text = "رمز عبور:",
                Location = new Point(30, 130),
                Size = new Size(100, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtPassword = new TextBox
            {
                Location = new Point(140, 130),
                Size = new Size(200, 25),
                UseSystemPasswordChar = true
            };

            // Login Button
            btnLogin = new Button
            {
                Text = "ورود",
                Location = new Point(230, 200),
                Size = new Size(110, 35),
                BackColor = Color.LightSeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            // Back Button
            btnBack = new Button
            {
                Text = "بازگشت",
                Location = new Point(110, 200),
                Size = new Size(110, 35),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += BtnBack_Click;

            // Add Controls to Form
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblPhoneNumber);
            this.Controls.Add(txtPhoneNumber);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnBack);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string phone = txtPhoneNumber.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("لطفاً تمامی فیلدها را پر کنید.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Authenticate user via CustomerService
                bool isAuthenticated = _customerService.Login(phone, password);

                if (isAuthenticated)
                {
                    MessageBox.Show("ورود با موفقیت انجام شد.", "موفقیت", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Route to the Home Form (Program.cs setup will handle tracking state)
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("شماره موبایل یا رمز عبور اشتباه است.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطایی در ورود رخ داد: {ex.Message}", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
