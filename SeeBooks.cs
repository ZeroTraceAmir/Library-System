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
        private TextBox _txtSearch;
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
            JsonNotificationRepository notificationRepository = new JsonNotificationRepository(
                store
            );
            JsonDebtRepository debtRepository = new JsonDebtRepository(store);
            JsonCustomerRepository customerRepository = new JsonCustomerRepository(store);

            bookService = new BookService(bookRepository);
            loanService = new LoanService(
                loanRepository,
                bookRepository,
                debtRepository,
                customerRepository
            );
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

            FlowLayoutPanel topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 50,
                Padding = new Padding(10),
                BackColor = ColorTranslator.FromHtml("#40404d"),
            };

            _txtSearch = new TextBox
            {
                Font = new Font("Vazir", 11F),
                Width = 200,
                PlaceholderText = "جستجو...",
                BackColor = ColorTranslator.FromHtml("#252836"),
                ForeColor = Color.White,
            };
            _txtSearch.TextChanged += (s, e) => LoadBooks();

            topPanel.Controls.Add(_txtSearch);

            dgvBooks = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            };

            btnBorrow = new Button
            {
                Text = "قرض گرفتن",
                Dock = DockStyle.Bottom,
                BackColor = ColorTranslator.FromHtml("#00ff9c"),
                ForeColor = ColorTranslator.FromHtml("#111520"),
                Font = new Font("Vazir", 11F, FontStyle.Bold),
                Height = 50,
            };

            btnReserve = new Button
            {
                Text = "رزرو کردن",
                Dock = DockStyle.Bottom,
                BackColor = ColorTranslator.FromHtml("#00ff9c"),
                ForeColor = ColorTranslator.FromHtml("#111520"),
                Font = new Font("Vazir", 11F, FontStyle.Bold),
                Height = 50,
            };

            btnBack = new Button
            {
                Text = "بازگشت",
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = ColorTranslator.FromHtml("#00ff9c"),
                ForeColor = ColorTranslator.FromHtml("#111520"),
                Font = new Font("Vazir", 11F, FontStyle.Bold),
                DialogResult = DialogResult.Cancel,
            };

            dgvBooks.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#1f293d"); // Choose your color
            dgvBooks.DefaultCellStyle.ForeColor = Color.White; // Text color
            dgvBooks.DefaultCellStyle.Font = new Font("Vazir", 9F);
            dgvBooks.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#00ff9c"); // Highlight color when clicked
            dgvBooks.DefaultCellStyle.SelectionForeColor = ColorTranslator.FromHtml("#111520");

            // Enable headers visual styles if you haven't already (important for header colors to show up)
            dgvBooks.EnableHeadersVisualStyles = false;
            dgvBooks.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#00ff9c");
            dgvBooks.ColumnHeadersDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#111520");
            dgvBooks.ColumnHeadersDefaultCellStyle.Font = new Font("Vazir", 9F, FontStyle.Bold);
            dgvBooks.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            btnBorrow.Click += BtnBorrow_Click;
            btnReserve.Click += BtnReserve_Click;

            Controls.Add(dgvBooks);
            Controls.Add(topPanel);
            Controls.Add(btnBorrow);
            Controls.Add(btnReserve);
            Controls.Add(btnBack);
        }

        private void LoadBooks()
        {
            dgvBooks.DataSource = null;
            dgvBooks.DataSource = bookService[_txtSearch.Text.Trim()];
        }

        private void BtnBorrow_Click(object? sender, EventArgs e)
        {
            if (dgvBooks.CurrentRow == null)
                return;

            Book book = (Book)dgvBooks.CurrentRow.DataBoundItem;

            try
            {
                loanService.BorrowBook(book, customer.Id);
                bookService.UpdateBook(book);

                MessageBox.Show("کتاب با موفقیت امانت گرفته شد");

                LoadBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
