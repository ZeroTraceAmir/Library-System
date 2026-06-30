using System;
using System.Windows.Forms;
using library_system.Models;
using library_system.Repositories;
using library_system.Services;

namespace library_system
{
    public class SeeBooks : Form
    {
        private readonly Customer customer;

        private DataGridView dgvBooks;
        private Button btnBorrow;
        private Button btnReserve;
        private Button btnBack;

        private readonly BookService bookService;
        private readonly LoanService loanService;
        private readonly ReservationService reservationService;
        private readonly NotificationService notificationService;

        public SeeBooks(Customer customer)
        {
            this.customer = customer;

            JsonDataStore store = new JsonDataStore();

            JsonBookRepository bookRepository = new JsonBookRepository(store);
            JsonLoanRepository loanRepository = new JsonLoanRepository(store);
            JsonReservationRepository reservationRepository = new JsonReservationRepository(store);
            JsonNotificationRepository notificationRepository = new JsonNotificationRepository(store);

            bookService = new BookService(bookRepository);
            loanService = new LoanService(loanRepository);
            reservationService = new ReservationService(reservationRepository);
            notificationService = new NotificationService(notificationRepository);

            reservationService.BookReserved += (book, _, customerId) =>
                notificationService.CreateReservationConfirmedNotification(customerId, book.Id);

            loanService.BookBorrowed += (book, _, customerId) =>
                notificationService.CreateBookBorrowedNotification(customerId, book.Id);

            InitializeComponent();
            LoadBooks();
        }

        private void InitializeComponent()
        {
            Text = "کتاب ها";
            WindowState = FormWindowState.Maximized;

            dgvBooks = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            btnBorrow = new Button
            {
                Text = "قرض گرفتن",
                Dock = DockStyle.Bottom,
                Height = 50
            };

            btnReserve = new Button
            {
                Text = "رزرو کردن",
                Dock = DockStyle.Bottom,
                Height = 50
            };

            btnBack = new Button
            {
                Text = "بازگشت",
                Dock = DockStyle.Bottom,
                Height = 50,
                DialogResult = DialogResult.Cancel
            };

            btnBorrow.Click += BtnBorrow_Click;
            btnReserve.Click += BtnReserve_Click;

            Controls.Add(dgvBooks);
            Controls.Add(btnBorrow);
            Controls.Add(btnReserve);
            Controls.Add(btnBack);
        }

        private void LoadBooks()
        {
            dgvBooks.DataSource = null;
            dgvBooks.DataSource = bookService.GetAllBooks();
        }

        private void BtnBorrow_Click(object? sender, EventArgs e)
        {
            if (dgvBooks.CurrentRow == null)
                return;

            Book book = (Book)dgvBooks.CurrentRow.DataBoundItem;

            loanService.BorrowBook(book, customer.Id);
            bookService.UpdateBook(book);

            MessageBox.Show("کتاب با موفقیت امانت گرفته شد");

            LoadBooks();
        }

        private void BtnReserve_Click(object? sender, EventArgs e)
        {
            if (dgvBooks.CurrentRow == null)
                return;

            Book book = (Book)dgvBooks.CurrentRow.DataBoundItem;

            reservationService.ReserveBook(book, customer.Id);

            MessageBox.Show("کتاب با موفقیت رزرو شد");

            LoadBooks();
        }
    }

}
