using System.Linq;
using System.Windows.Forms;
using library_system.Models;
using library_system.Repositories;
using library_system.Services;

namespace library_system
{
    public class MyReservations : Form
    {
        private DataGridView dgvReservations;
        private Button btnCancelReservation;
        private Button btnBack;
        private readonly CustomerService _customerService;
        private readonly ReservationService _reservationService;
        private readonly NotificationService _notificationService;
        private readonly BookService _bookService;

        public MyReservations()
        {
            JsonDataStore store = new JsonDataStore();
            _customerService = new CustomerService(new JsonCustomerRepository(store));
            _reservationService = new ReservationService(new JsonReservationRepository(store));
            _notificationService = new NotificationService(new JsonNotificationRepository(store));
            _bookService = new BookService(new JsonBookRepository(store));

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
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = ColorTranslator.FromHtml("#111520"),
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

            dgvReservations.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#1f293d"); // Choose your color
            dgvReservations.DefaultCellStyle.ForeColor = Color.White; // Text color
            dgvReservations.DefaultCellStyle.Font = new Font("Vazir", 9F);
            dgvReservations.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#00ff9c"); // Highlight color when clicked
            dgvReservations.DefaultCellStyle.SelectionForeColor = ColorTranslator.FromHtml("#111520");
        }

        private void LoadReservations()
        {
            Customer? customer = _customerService.GetLoggedInCustomer();

            if (customer == null)
                return;

            List<Reservation> reservations = _reservationService.GetReservationsByCustomerId(customer.Id);
            Dictionary<int, Book> books = _bookService.GetAllBooks().ToDictionary(b => b.Id);

            List<ReservationDisplay> reservationData = reservations
                .Select(r => new ReservationDisplay
                {
                    Id = r.Id,
                    BookName = books.TryGetValue(r.BookId, out Book book) ? book.Title : "نامشخص",
                    ReservationDate = r.ReservationDate,
                    Status = r.IsActive ? "فعال" : "لغو شده",
                })
                .ToList();

            dgvReservations.DataSource = reservationData;

            dgvReservations.Columns["Id"]!.Visible = false;
            dgvReservations.Columns["BookName"].HeaderText = "نام کتاب";
            dgvReservations.Columns["ReservationDate"].HeaderText = "تاریخ رزرو";
            dgvReservations.Columns["Status"].HeaderText = "وضعیت";
        }

        private void BtnCancelReservation_Click(object? sender, EventArgs e)
        {
            if (dgvReservations.CurrentRow == null)
            {
                MessageBox.Show("ابتدا یک رزرو را انتخاب کنید");
                return;
            }

            int reservationId = Convert.ToInt32(dgvReservations.CurrentRow.Cells["Id"].Value);

            Reservation? reservation = _reservationService.GetReservationById(reservationId);

            _reservationService.CancelReservation(reservationId);

            if (reservation != null)
            {
                _notificationService.CreateReservationCancelledNotification(
                    reservation.CustomerId, reservation.BookId);
            }

            LoadReservations();
            MessageBox.Show("رزرو با موفقیت لغو شد");
        }

        private record ReservationDisplay
        {
            public int Id { get; init; }
            public string BookName { get; init; }
            public DateTime ReservationDate { get; init; }
            public string Status { get; init; }
        }
    }
}
