using System;
using System.Drawing;
using System.Windows.Forms;
using library_system.Models;

namespace library_system
{
    public class AdminPanel : Form
    {
        private readonly User _user;

        public AdminPanel(User user)
        {
            _user = user;
            this.Text = "Admin Panel";
            this.WindowState = FormWindowState.Maximized;
            BuildHeader(user.Name, user.Number, "مدیر");
            BuildMenu();
        }

        private void BuildHeader(string name, string phone, string role)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(52, 73, 94)
            };

            var btnProfile = new Button
            {
                Text = "پروفایل",
                Dock = DockStyle.Left,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Font = new Font("Tahoma", 10F, FontStyle.Bold),
                Width = 100,
            };
            btnProfile.Click += BtnProfile_Click;

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(15),
            };

            void AddLabel(string text, float fontSize, bool bold, Color color)
            {
                flow.Controls.Add(new Label
                {
                    Text = text,
                    Font = new Font("Tahoma", fontSize, bold ? FontStyle.Bold : FontStyle.Regular),
                    ForeColor = color,
                    AutoSize = true,
                    Margin = new Padding(20, 15, 0, 0)
                });
            }

            AddLabel(role, 11F, true, Color.FromArgb(46, 204, 113));
            AddLabel(phone, 10F, false, Color.White);
            AddLabel(name, 11F, true, Color.White);

            panel.Controls.Add(flow);
            panel.Controls.Add(btnProfile);
            Controls.Add(panel);
        }

        private void BuildMenu()
        {
            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(50, 40, 50, 0),
            };

            void AddButton(string text, EventHandler onClick)
            {
                var btn = new Button
                {
                    Text = text,
                    Font = new Font("Tahoma", 11F),
                    BackColor = Color.FromArgb(52, 73, 94),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    AutoSize = true,
                    Padding = new Padding(40, 15, 40, 15),
                    Margin = new Padding(0, 0, 0, 15),
                };
                btn.Click += onClick;
                flow.Controls.Add(btn);
            }

            AddButton("دیدن همه کارمندان کتابخانه", (s, e) =>
            {
                this.Hide();
                new SeeAllEmployees().ShowDialog();
                this.Show();
            });
            AddButton("دیدن همه مشتریان کتابخانه", (s, e) =>
            {
                this.Hide();
                new SeeAllCustomers().ShowDialog();
                this.Show();
            });
            AddButton("اضافه کردن کارمند به کتاب خانه", (s, e) =>
            {
                this.Hide();
                new AddEmployee().ShowDialog();
                this.Show();
            });

            Controls.Add(flow);
        }

        private void BtnProfile_Click(object? sender, EventArgs e)
        {
            var pf = new Profile(_user.Name, _user.Number, "مدیر", true);
pf.ShowDialog(this);
        }
    }
}
