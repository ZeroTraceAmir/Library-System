using System;
using System.Drawing;
using System.Windows.Forms;
using library_system.Enums;
using library_system.Interfaces;
using library_system.Models;
using library_system.Repositories;
using library_system.Services;

namespace library_system
{
    public class Form1 : Form
    {
        public Form1()
        {
            // Form settings
            Text = "کتابخانه";
            Size = new Size(1360, 720);
            StartPosition = FormStartPosition.CenterScreen;
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            BackColor = ColorTranslator.FromHtml("#111520");

            // Header
            Label header = new Label
            {
                Text = "سامانه امانت الکترونیک",
                // Font = new Font("Tahoma", 24F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 80,
                ForeColor = Color.White,
                Font = new Font("Vazir", 30F, FontStyle.Bold),
            };

            // Paragraph
            Label paragraph = new Label
            {
                Text = "برای استفاده از سامانه، وارد شوید",
                // Font = new Font("Tahoma", 12F, FontStyle.Regular),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 50,
                Font = new Font("Vazir", 12F, FontStyle.Regular),
                ForeColor = Color.White,
            };

            Panel buttonPanel = new Panel { Dock = DockStyle.Top, Height = 80 };

            Button btnRegister = new Button
            {
                Text = "ثبت نام",
                Font = new Font("Vazir", 11F),
                Size = new Size(140, 45),
                BackColor = ColorTranslator.FromHtml("#00ff9c"),
                ForeColor = ColorTranslator.FromHtml("#111520"),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(15, 0, 30, 0),
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;

            Button btnLogin = new Button
            {
                Text = "ورود",
                Font = new Font("Vazir", 11F),
                Size = new Size(140, 45),
                BackColor = ColorTranslator.FromHtml("#00ff9c"),
                ForeColor = ColorTranslator.FromHtml("#111520"),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(15, 0, 15, 0),
            };
            btnRegister.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#02a9af");

            btnLogin.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#02a9af");
            btnRegister.Location = new Point((buttonPanel.Width / 2) - btnRegister.Width - 10, 20);

            btnLogin.Location = new Point((buttonPanel.Width / 2) + 10, 20);

            buttonPanel.Resize += (s, e) =>
            {
                btnRegister.Location = new Point(
                    (buttonPanel.Width / 2) - btnRegister.Width - 10,
                    20
                );

                btnLogin.Location = new Point((buttonPanel.Width / 2) + 10, 20);
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            // Center the buttons by anchoring the panel content
            buttonPanel.Controls.Add(btnRegister);
            buttonPanel.Controls.Add(btnLogin);

            // Add controls (reverse order because of DockStyle.Top stacking)
            Controls.Add(buttonPanel);
            Controls.Add(paragraph);
            Controls.Add(header);
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            JsonDataStore dataStore = new JsonDataStore();
            ICustomerRepository customerRepository = new JsonCustomerRepository(dataStore);
            CustomerService customerService = new CustomerService(customerRepository);

            RegisterForm registerForm = new RegisterForm(customerService);
            registerForm.Show();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            JsonDataStore dataStore = new JsonDataStore();
            ICustomerRepository customerRepository = new JsonCustomerRepository(dataStore);
            CustomerService customerService = new CustomerService(customerRepository);
            IUserRepository userRepository = new JsonUserRepository(dataStore);
            UserService userService = new UserService(userRepository);

            LoginForm loginForm = new LoginForm(customerService, userService);
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                Form nextForm;
                if (loginForm.LoggedInUserRole == Enums.UserStatus.admin)
                {
                    User user = userService.GetLoggedInUser()!;
                    nextForm = new AdminPanel(user);
                }
                else if (loginForm.LoggedInUserRole == Enums.UserStatus.staff)
                {
                    User user = userService.GetLoggedInUser()!;
                    nextForm = new StaffPanel(user);
                }
                else
                {
                    Customer customer = customerService.GetLoggedInCustomer()!;
                    nextForm = new Home(customer);
                }

                nextForm.Show();
                this.Hide();
            }
        }
    }
}
