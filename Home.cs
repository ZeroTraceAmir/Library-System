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
            BuildHeader(customer.Name, customer.Number, customer.GetRoleLabel());
            BuildMenu();
        }

        private void BuildHeader(string name, string phone, string role)
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(52, 73, 94),
            };

            Button btnProfile = new Button
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
            panel.Controls.Add(btnProfile);
            Controls.Add(panel);
        }

        private void BuildMenu()
        {
            FlowLayoutPanel flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(50, 40, 50, 0),
            };

            void AddButton(string text, EventHandler onClick)
            {
                Button btn = new Button
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

            AddButton(
                "دیدن کتاب های",
                (s, e) =>
                {
                    this.Hide();
                    new SeeBooks(_customer).ShowDialog();
                    this.Show();
                }
            );
            AddButton(
                "دیدن کتاب هایی که رزرو کردید",
                (s, e) =>
                {
                    this.Hide();
                    new MyReservations().ShowDialog();
                    this.Show();
                }
            );
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
        }

        private void BtnProfile_Click(object? sender, EventArgs e)
        {
            Profile pf = new Profile(_customer.Name, _customer.Number, _customer.GetRoleLabel(), false);
pf.ShowDialog(this);
        }
    }
}
