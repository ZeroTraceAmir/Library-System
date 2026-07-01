using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using library_system.Enums;
using library_system.Models;
using library_system.Repositories;
using library_system.Services;

namespace library_system
{
    public class Notifications : Form
    {
        private readonly Customer _customer;
        private FlowLayoutPanel flowPanel;

        public Notifications(Customer customer)
        {
            _customer = customer;
            InitializeComponent();
            LoadNotifications();
        }

        private void InitializeComponent()
        {
            this.Text = "اعلان ها";
            this.Size = new Size(500, 720);
            this.StartPosition = FormStartPosition.CenterParent;

            flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(10),
            };

            Button btnBack = new Button
            {
                Text = "بازگشت",
                Dock = DockStyle.Bottom,
                Height = 50,
                DialogResult = DialogResult.Cancel,
                Font = new Font("Tahoma", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
            };

            Controls.Add(flowPanel);
            Controls.Add(btnBack);
        }

        private void LoadNotifications()
        {
            JsonDataStore store = new JsonDataStore();
            JsonNotificationRepository repository = new JsonNotificationRepository(store);
            NotificationService service = new NotificationService(repository);

            JsonLoanRepository loanRepo = new JsonLoanRepository(store);
            LoanService loanService = new LoanService(loanRepo);
            List<Loan> customerLoans = loanService.GetLoansByCustomerId(_customer.Id);
            service.CheckOverdueAndReminders(customerLoans);

            List<Notification> notifications = service.GetNotificationsForCustomer(_customer.Id);

            foreach (Notification notification in notifications)
            {
                flowPanel.Controls.Add(CreateNotificationBox(notification));
            }

            if (notifications.Count == 0)
            {
                Label emptyLabel = new Label
                {
                    Text = "اعلانی وجود ندارد",
                    Font = new Font("Tahoma", 12F),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Margin = new Padding(10),
                };
                flowPanel.Controls.Add(emptyLabel);
            }
        }

        private Panel CreateNotificationBox(Notification notification)
        {
            Color boxColor = notification.Type switch
            {
                NotificationType.Overdue => Color.FromArgb(231, 76, 60),
                NotificationType.ReturnReminder => Color.FromArgb(243, 156, 18),
                NotificationType.ReservationConfirmed => Color.FromArgb(46, 204, 113),
                NotificationType.ReservationCancelled => Color.FromArgb(149, 165, 166),
                NotificationType.BookBorrowed => Color.FromArgb(41, 128, 185),
                _ => Color.FromArgb(52, 73, 94),
            };

            Panel box = new Panel
            {
                Width = 460,
                Height = 80,
                BackColor = boxColor,
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(15),
            };

            Label messageLabel = new Label
            {
                Text = notification.GetMessage(),
                Font = new Font("Tahoma", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Width = 430,
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft,
            };

            Label dateLabel = new Label
            {
                Text = notification.CreatedAt.ToString("yyyy/MM/dd HH:mm"),
                Font = new Font("Tahoma", 9F),
                ForeColor = Color.FromArgb(255, 255, 255, 200),
                AutoSize = false,
                Width = 430,
                Height = 20,
                TextAlign = ContentAlignment.MiddleLeft,
                Top = 35,
            };

            box.Controls.Add(messageLabel);
            box.Controls.Add(dateLabel);

            return box;
        }
    }
}
