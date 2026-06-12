using System.Windows.Forms;
using library_system.Repositories;
using library_system.Services;
using library_system.Models;

namespace library_system
{
    public class MyReservations : Form
    {
        private DataGridView dgvReservations;
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

            btnBack = new Button
            {
                Text = "بازگشت",
                Dock = DockStyle.Bottom,
                Height = 50,
                DialogResult = DialogResult.Cancel
            };

            Controls.Add(dgvReservations);
            Controls.Add(btnBack);
        }

        private void LoadReservations()
        {
            JsonDataStore store = new JsonDataStore();

            var customerRepository = new JsonCustomerRepository(store);
            var customerService = new CustomerService(customerRepository);

            Customer? customer = customerService.GetLoggedInCustomer();

            if (customer == null)
                return;

            var reservationRepository =
                new JsonReservationRepository(store);

            var reservationService =
                new ReservationService(reservationRepository);

            dgvReservations.DataSource =
                reservationService.GetReservationsByCustomerId(customer.Id);
        }
    }

}
