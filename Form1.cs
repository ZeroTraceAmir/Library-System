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
            Size = new Size(500, 350);
            StartPosition = FormStartPosition.CenterScreen;
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            BackColor = Color.White;

            // Header
            Label header = new Label
            {
                Text = "کتابخانه",
                Font = new Font("Tahoma", 24F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 80,
            };

            // Paragraph
            Label paragraph = new Label
            {
                Text = "برای استفاده از کتابخانه، وارد شوید",
                Font = new Font("Tahoma", 12F, FontStyle.Regular),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 50,
            };

            // Buttons panel
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(0, 20, 0, 0),
            };

            Button btnRegister = new Button
            {
                Text = "ثبت نام",
                Font = new Font("Tahoma", 11F),
                Size = new Size(140, 45),
                BackColor = Color.FromArgb(46, 134, 193),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(15, 0, 30, 0),
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;

            Button btnLogin = new Button
            {
                Text = "ورود",
                Font = new Font("Tahoma", 11F),
                Size = new Size(140, 45),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(15, 0, 15, 0),
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
