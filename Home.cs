using System;
using System.Drawing;
using System.Windows.Forms;
using library_system.Models;

namespace library_system
{
    public class Home : Form
    {
        private readonly Customer _customer;

        public Home(Customer customer)
        {
            _customer = customer;
            this.Text = "Home";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = ColorTranslator.FromHtml("#111520");
            BuildHeader(customer.Name, customer.Number, customer.GetRoleLabel());
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
                Width = 15, // This acts as your margin/gap size
            };
            Button notifBtn = new Button
            {
                Text = "اعلان ها",
                Dock = DockStyle.Left,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                BackColor = Color.FromArgb(52, 88, 94),
                ForeColor = Color.White,
                Font = new Font("Vazir", 10F, FontStyle.Bold),
                Width = 80,
            };
            notifBtn.Click += (s, e) => new Notifications(_customer).ShowDialog(this);

            // FlowLayoutPanel flow = new FlowLayoutPanel
            // {
            //     Dock = DockStyle.Fill,
            //     FlowDirection = FlowDirection.RightToLeft,
            //     Padding = new Padding(15),
            // };

            // void AddLabel(string text, float fontSize, bool bold, Color color)
            // {
            //     flow.Controls.Add(
            //         new Label
            //         {
            //             Text = text,
            //             Font = new Font(
            //                 "Vazir",
            //                 fontSize,
            //                 bold ? FontStyle.Bold : FontStyle.Regular
            //             ),
            //             ForeColor = color,
            //             AutoSize = true,
            //             Margin = new Padding(20, 15, 0, 0),
            //         }
            //     );
            // }

            // AddLabel(role, 11F, true, Color.FromArgb(46, 204, 113));
            // AddLabel(phone, 10F, false, Color.White);
            // AddLabel(name, 11F, true, Color.White);

            // panel.Controls.Add(flow);
            Label greeting = new Label
            {
                Text = $"❤️به سامانه امانت الکترونیک خوش امدی !{name} سلام",
                Font = new Font("Vazir", 15F, FontStyle.Bold),
                ForeColor = Color.Wheat,
                Dock = DockStyle.Right,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
            };
            panel.Controls.Add(greeting);
            panel.Controls.Add(notifBtn);
            panel.Controls.Add(separator);
            panel.Controls.Add(btnProfile);
            Controls.Add(panel);
        }

        // private void BuildMenu()
        // {
        //     FlowLayoutPanel flow = new FlowLayoutPanel
        //     {
        //         Dock = DockStyle.Fill,
        //         FlowDirection = FlowDirection.RightToLeft,
        //         WrapContents = false,
        //         Padding = new Padding(50, 300, 50, 0),
        //     };

        //     void AddButton(string text, EventHandler onClick)
        //     {
        //         Button btn = new Button
        //         {
        //             Text = text,
        //             Font = new Font("Vazir", 11F,FontStyle.Bold),
        //             BackColor = ColorTranslator.FromHtml("#00ff9c"),
        //             ForeColor = ColorTranslator.FromHtml("#111520"),
        //             FlatStyle = FlatStyle.Flat,
        //             FlatAppearance = { BorderSize = 0 },
        //             AutoSize = true,
        //             Padding = new Padding(40, 15, 40, 15),
        //             Margin = new Padding(0, 0, 0, 100),
        //         };

        //         Panel secondSeparator = new Panel
        //         {
        //             Dock = DockStyle.Left,
        //             Width = 15, // This acts as your margin/gap size
        //         };
        //         btn.Click += onClick;
        //         flow.Controls.Add(secondSeparator);
        //         flow.Controls.Add(btn);
        //     }

        //     AddButton(
        //         "دیدن کتاب ها",
        //         (s, e) =>
        //         {
        //             this.Hide();
        //             new SeeBooks(_customer).ShowDialog();
        //             this.Show();
        //         }
        //     );
        //     AddButton(
        //         "دیدن کتاب هایی که رزرو کردید",
        //         (s, e) =>
        //         {
        //             this.Hide();
        //             new MyReservations().ShowDialog();
        //             this.Show();
        //         }
        //     );
        //     AddButton(
        //         "دیدن کتاب هایی که قرض گرفتید",
        //         (s, e) =>
        //         {
        //             this.Hide();
        //             new MyLoans().ShowDialog();
        //             this.Show();
        //         }
        //     );
        //     AddButton(
        //         "پرداخت بدهی",
        //         (s, e) =>
        //         {
        //             this.Hide();
        //             new PayDebt().ShowDialog();
        //             this.Show();
        //         }
        //     );

        //     Controls.Add(flow);
        // }
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

            AddButton(
                "دیدن کتاب ها",
                (s, e) =>
                {
                    this.Hide();
                    new SeeBooks(_customer).ShowDialog();
                    this.Show();
                }
            );
            // AddButton(
            //     "دیدن کتاب هایی که رزرو کردید",
            //     (s, e) =>
            //     {
            //         this.Hide();
            //         new MyReservations().ShowDialog();
            //         this.Show();
            //     }
            // );
            AddButton(
                "دیدن کتاب هایی که قرض گرفتید",
                (s, e) =>
                {
                    this.Hide();
                    new MyLoans().ShowDialog();
                    this.Show();
                }
            );
            AddButton(
                "پرداخت بدهی",
                (s, e) =>
                {
                    this.Hide();
                    new PayDebt().ShowDialog();
                    this.Show();
                }
            );

            Controls.Add(flow);

            // Center the flow panel horizontally based on the screen width
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int centerX = screenWidth / 2;
            flow.Location = new Point(centerX - (flow.Width / 2), flow.Location.Y);
        }

        private void BtnProfile_Click(object? sender, EventArgs e)
        {
            Profile pf = new Profile(
                _customer.Name,
                _customer.Number,
                _customer.GetRoleLabel(),
                false
            );
            pf.ShowDialog(this);
        }
    }
}
