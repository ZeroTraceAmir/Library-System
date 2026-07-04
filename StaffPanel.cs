using System;
using System.Drawing;
using System.Windows.Forms;
using library_system.Models;

namespace library_system
{
    public class StaffPanel : Form
    {
        private readonly User _user;

        public StaffPanel(User user)
        {
            _user = user;
            this.Text = "Staff Panel";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = ColorTranslator.FromHtml("#111520");
            BuildHeader(user.Name, user.Number, user.GetRoleLabel());
            BuildMenu();
        }

        private void BuildHeader(string name, string phone, string role)
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = ColorTranslator.FromHtml("#252836"),
            };

            Button btnProfile = new Button
            {
                Text = "پروفایل",
                Dock = DockStyle.Left,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(52, 73, 94),
                FlatAppearance = { BorderSize = 0 },
                ForeColor = Color.White,
                Font = new Font("Vazir", 10F, FontStyle.Bold),
                Width = 100,
            };
            btnProfile.Click += BtnProfile_Click;
            Panel separator = new Panel
            {
                Dock = DockStyle.Left,
                Width = 15,
            };

            Label greeting = new Label
            {
                Text = $"به سامانه امانت الکترونیک خوش امدی !{name} سلام",
                Font = new Font("Vazir", 15F, FontStyle.Bold),
                ForeColor = Color.Wheat,
                Dock = DockStyle.Right,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
            };
            panel.Controls.Add(greeting);
            panel.Controls.Add(separator);
            panel.Controls.Add(btnProfile);
            Controls.Add(panel);
        }

        private void BuildMenu()
        {
            FlowLayoutPanel flow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0, 300, 0, 0),
            };

            void AddButton(string text, EventHandler onClick)
            {
                Button btn = new Button
                {
                    Text = text,
                    Font = new Font("Vazir", 11F, FontStyle.Bold),
                    BackColor = ColorTranslator.FromHtml("#00ff9c"),
                    ForeColor = ColorTranslator.FromHtml("#111520"),
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    AutoSize = true,
                    Padding = new Padding(40, 15, 40, 15),
                    Margin = new Padding(0, 0, 0, 100),
                };

                Panel secondSeparator = new Panel { Dock = DockStyle.Left, Width = 15 };
                btn.Click += onClick;
                flow.Controls.Add(secondSeparator);
                flow.Controls.Add(btn);
            }

            AddButton("دیدن همه کتاب ها", (s, e) =>
            {
                this.Hide();
                new SeeAllBooks().ShowDialog();
                this.Show();
            });
            AddButton("دیدن همه مشتریان", (s, e) =>
            {
                this.Hide();
                new SeeAllCustomers().ShowDialog();
                this.Show();
            });
            AddButton("اضافه کردن کتاب", (s, e) =>
            {
                this.Hide();
                new AddBook().ShowDialog();
                this.Show();
            });

            Controls.Add(flow);

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int centerX = screenWidth / 2;
            flow.Location = new Point(centerX - (flow.Width / 2), flow.Location.Y);
        }

        private void BtnProfile_Click(object? sender, EventArgs e)
        {
            Profile pf = new Profile(_user.Name, _user.Number, _user.GetRoleLabel(), true);
            pf.ShowDialog(this);
        }
    }
}
