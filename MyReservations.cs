using System.Windows.Forms;
using library_system.Repositories;
using library_system.Services;
using library_system.Models;

namespace library_system
{
    public class MyReservations : Form
    {
        private DataGridView dgvReservations;
        private Button btnCancelReservation;
        private Button btnBack;

        public MyReservations()
        {
            InitializeComponent();
            LoadReservations();
        }

        private void InitializeComponent()
        {
            this.Text = "رزروهای من";
            this.WindowState = FormWindowState.Maximized;

            dgvReservations = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            btnCancelReservation = new Button
            {
                Text = "لغو رزرو",
                Dock = DockStyle.Bottom,
                Height = 50
            };

            btnCancelReservation.Click += BtnCancelReservation_Click;

            btnBack = new Button
            {
                Text = "بازگشت",
                Dock = DockStyle.Bottom,
                Height = 50,
                DialogResult = DialogResult.Cancel
            };

            Controls.Add(dgvReservations);
            Controls.Add(btnCancelReservation);
            Controls.Add(btnBack);
        }

        private void LoadReservations()
        {
            JsonDataStore store = new JsonDataStore();

            JsonCustomerRepository customerRepository = new JsonCustomerRepository(store);
            CustomerService customerService = new CustomerService(customerRepository);

            Customer? customer = customerService.GetLoggedInCustomer();

            if (customer == null)
                return;

            JsonReservationRepository reservationRepository =
                new JsonReservationRepository(store);

            ReservationService reservationService =
                new ReservationService(reservationRepository);

            dgvReservations.DataSource =
                reservationService.GetReservationsByCustomerId(customer.Id);
        }

        private void BtnCancelReservation_Click(object? sender, EventArgs e)
        {
            if (dgvReservations.CurrentRow == null)
            {
                MessageBox.Show("ابتدا یک رزرو را انتخاب کنید");
                return;
            }

            int reservationId =
                Convert.ToInt32(
                    dgvReservations.CurrentRow.Cells["Id"].Value);

            JsonDataStore store = new JsonDataStore();

            JsonReservationRepository reservationRepository =
                new JsonReservationRepository(store);

            ReservationService reservationService =
                new ReservationService(reservationRepository);

            Reservation? reservation = reservationService.GetReservationById(reservationId);

            reservationService.CancelReservation(reservationId);

            if (reservation != null)
            {
                JsonNotificationRepository notificationRepository =
                    new JsonNotificationRepository(store);

                NotificationService notificationService =
                    new NotificationService(notificationRepository);

                notificationService.CreateReservationCancelledNotification(
                    reservation.CustomerId, reservation.BookId);
            }

            LoadReservations();

            MessageBox.Show("رزرو با موفقیت لغو شد");

        }
    }

}
