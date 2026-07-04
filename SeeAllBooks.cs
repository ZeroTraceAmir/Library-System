using System; // Provides fundamental classes like String, Int32, EventArgs, etc.
using System.Drawing; // Provides the Font class used for styling UI controls (e.g., new Font("Vazir", 11F)).
using System.Linq; // Provides LINQ extension methods: .Select(), .Where(), .Distinct(), .OrderBy(), .ToList().
using System.Windows.Forms; // Provides all WinForms controls: Form, DataGridView, ComboBox, TextBox, FlowLayoutPanel, etc.
using library_system.Services; // Provides BookService, which contains business logic for book operations.

namespace library_system // This project's root namespace (defined in the .csproj as RootNamespace).
{
    public class SeeAllBooks : Form // Inherits from System.Windows.Forms.Form, making this a window.
    {
        // ── Fields ──────────────────────────────────────────────────────────────
        // BookService is the business-logic layer that talks to the repository layer.
        // It is instantiated in the constructor and stored here so all methods can use it.
        private readonly BookService _bookService;

        // DataGridView is the WinForms control that displays tabular data (rows/columns).
        // It is filled dynamically by setting its .DataSource property.
        private readonly DataGridView _grid;

        // ComboBox lets the user pick a genre to filter the book list.
        // Its SelectedIndexChanged event triggers a re-filter.
        private readonly ComboBox _cmbFilter;

        // TextBox for the user to type a search keyword (searches book title & author).
        // Its TextChanged event triggers a re-filter.
        private readonly TextBox _txtSearch;

        // ── Constructor ──────────────────────────────────────────────────────────
        // Called when a new SeeAllBooks instance is created (e.g., "new SeeAllBooks()").
        // Sets up the form layout, controls, data store, and loads initial data.
        public SeeAllBooks()
        {
            // --- Data Layer Setup ---
            // JsonDataStore is the single file I/O helper (reads/writes JSON files in Data/).
            // It determines the base directory and file paths for all JSON data files.
            Repositories.JsonDataStore store = new Repositories.JsonDataStore();

            // BookService is the service-layer class that contains business logic
            // (e.g., GetAllBooks(), GetBookByTitle(), etc.). It receives a repository
            // that handles the actual JSON serialization/deserialization.
            // JsonBookRepository implements IBookRepository and uses JsonDataStore
            // to read/write Data/books.json.
            _bookService = new BookService(new Repositories.JsonBookRepository(store));

            // --- Form-Level Settings ---
            // The Persian (Farsi) title displayed in the window's title bar.
            this.Text = "دیدن همه کتاب ها";

            // Makes the form fill the entire screen (maximized window).
            this.WindowState = FormWindowState.Maximized;

            // ── Top Toolbar (FlowLayoutPanel) ────────────────────────────────────
            // A panel docked to the top of the form that holds the search TextBox and genre ComboBox.
            // FlowLayoutPanel automatically arranges its child controls left-to-right or right-to-left.
            FlowLayoutPanel topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top, // Anchors to the top edge of the form, spans full width.
                FlowDirection = FlowDirection.RightToLeft, // Controls stack from right to left (RTL layout for Persian UI).
                Height = 50, // Fixed height for the toolbar row.
                Padding = new Padding(10), // 10px internal margin on all sides.
                BackColor = ColorTranslator.FromHtml("#40404d"),
            };

            // ── Search TextBox ───────────────────────────────────────────────────
            // The user types here to filter books by title or author.
            _txtSearch = new TextBox
            {
                Font = new Font("Vazir", 11F), // Font family "Vazir", size 11pt (matching the app's general font).
                Width = 200, // Fixed width in pixels.
                PlaceholderText = "جستجو...", // Watermark text shown inside the box when it is empty.
                BackColor = ColorTranslator.FromHtml("#252836"),
                ForeColor = Color.White,
            };
            // Every time the user types or deletes a character, TextChanged fires → RefreshGrid() re-filters.
            _txtSearch.TextChanged += (s, e) => RefreshGrid();

            // ── Genre Filter ComboBox ────────────────────────────────────────────
            // Drop-down list populated with "همه" (All) followed by every distinct genre found in the book data.
            _cmbFilter = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList, // User cannot type custom text; they must pick from the list.
                Font = new Font("Vazir", 11F), // Same font style as the search box.
                Width = 150, // Fixed width in pixels.
                BackColor = ColorTranslator.FromHtml("#252836"),
                ForeColor = Color.WhiteSmoke,
            };
            // When the user selects a different genre, SelectedIndexChanged fires → RefreshGrid() re-filters.
            _cmbFilter.SelectedIndexChanged += (s, e) => RefreshGrid();

            // Add the controls to the toolbar in reverse order because FlowDirection is RightToLeft:
            // The ComboBox appears on the right, and the TextBox appears to its left.
            topPanel.Controls.Add(_cmbFilter);
            topPanel.Controls.Add(_txtSearch);

            // ── DataGridView (Book Table) ────────────────────────────────────────
            // The main grid that shows the list of books with columns for title, author, genre, etc.
            _grid = new DataGridView
            {
                Dock = DockStyle.Fill, // Fills all remaining space between topPanel and the bottom button.
                Font = new Font("Vazir", 10F), // Slightly smaller font (10pt) for the table cells.
                AllowUserToAddRows = false, // Removes the blank "new row" row at the bottom.
                AllowUserToDeleteRows = false, // Prevents the user from deleting rows via the grid UI.
                ReadOnly = true, // All cells are read-only (data modification is done through other forms).
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, // Each column stretches to fill the grid width equally.
                RightToLeft = RightToLeft.Yes, // Columns align from right to left (Persian UI convention).
                BackgroundColor = ColorTranslator.FromHtml("#111520"),
            };

            _grid.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#1f293d"); // Choose your color
            _grid.DefaultCellStyle.ForeColor = Color.White; // Text color
            _grid.DefaultCellStyle.Font = new Font("Vazir", 9F);
            _grid.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#00ff9c"); // Highlight color when clicked
            _grid.DefaultCellStyle.SelectionForeColor = ColorTranslator.FromHtml("#111520");

            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#00ff9c");
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#111520");
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Vazir", 9F, FontStyle.Bold);
            _grid.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            // Add the toolbar (topPanel) and the data grid (_grid) to the form's Controls collection.
            // Order matters for z-order but with Dock styles the layout is resolved by docking.
            Controls.Add(_grid); // DataGridView fills the remaining space between toolbar and back button.
            Controls.Add(topPanel); // Toolbar docked to the top.

            // ── Bottom "Back" Button ─────────────────────────────────────────────
            // A simple button docked to the bottom of the form that closes this dialog.
            Controls.Add(
                new Button
                {
                    Text = "بازگشت", // Persian for "Back".
                    Dock = DockStyle.Bottom, // Anchored to the bottom edge, spans full width.
                    Height = 50, // Tall enough to be easily clickable.
                    DialogResult = DialogResult.Cancel, // When clicked (or Enter pressed), the form closes with Cancel result.
                    BackColor = ColorTranslator.FromHtml("#00ff9c"),
                    ForeColor = ColorTranslator.FromHtml("#111520"),
                    Font = new Font("Vazir", 11F, FontStyle.Bold),
                }
            );

            // ── Load Data ────────────────────────────────────────────────────────
            // Populates the genre dropdown with distinct genres from the book data.
            LoadGenres();
            // Fills the DataGridView with all books (respecting the current filter/search state).
            RefreshGrid();
        }

        // ── LoadGenres ──────────────────────────────────────────────────────────
        // Reads all books from BookService, extracts distinct genres,
        // and populates _cmbFilter with "همه" (All) followed by each genre.
        private void LoadGenres()
        {
            // First item: "همه" (All). Index 0 means "no genre filter".
            _cmbFilter.Items.Add("همه");

            // GetAllBooks() is defined in BookService (Services/BookService.cs).
            // It calls JsonBookRepository.GetAll() (Repositories/JsonBookRepository.cs)
            // which deserializes Data/books.json into a List<Book>.
            //
            // The LINQ pipeline below:
            //   .Select(b => b.Genre)           → pull just the Genre string from each Book.
            //   .Where(g => !string.IsNullOrWhiteSpace(g)) → skip null/empty/whitespace genres.
            //   .Distinct(...)                   → keep only unique genre names (case-insensitive).
            //   .OrderBy(g => g)                 → sort alphabetically.
            //   .ToList()                        → materialize into a List<string>.
            List<string> genres = _bookService
                .GetAllBooks()
                .Select(b => b.Genre)
                .Where(g => !string.IsNullOrWhiteSpace(g))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(g => g)
                .ToList();

            // Add each genre as a separate item in the ComboBox.
            foreach (string genre in genres)
                _cmbFilter.Items.Add(genre);

            // Default selection: "همه" (index 0) — show all books initially.
            _cmbFilter.SelectedIndex = 0;
        }

        // ── RefreshGrid ─────────────────────────────────────────────────────────
        // Re-queries BookService for all books, applies the current genre filter
        // and search text filter, then binds the result to the DataGridView.
        private void RefreshGrid()
        {
            // Grab the search text, trim leading/trailing spaces, convert to lowercase
            // for a case-insensitive comparison with book titles and authors.
            string search = _txtSearch.Text.Trim().ToLower();

            // LINQ query over all books returned by BookService.GetAllBooks().
            var books = _bookService
                .GetAllBooks() // List<Book> from JSON file via BookService → JsonBookRepository.
                .Where(b => // Filter the book list.
                {
                    // ── Genre Filter ─────────────────────────────────────────────
                    // If the user selected a specific genre (index > 0 means not "همه"),
                    // check whether this book's Genre matches the selected genre.
                    if (_cmbFilter.SelectedIndex > 0)
                    {
                        // Retrieve the currently selected genre string from the ComboBox.
                        // .ToString() is safe here because the items are all strings.
                        string selectedGenre = _cmbFilter.SelectedItem.ToString();
                        // Case-insensitive comparison. If the genres differ, exclude this book.
                        if (!b.Genre.Equals(selectedGenre, StringComparison.OrdinalIgnoreCase))
                            return false; // Book does not match genre filter → skip it.
                    }

                    // ── Search-Text Filter ───────────────────────────────────────
                    // If the user typed something in the search box, check if the
                    // book's Title or Author contains the search string (case-insensitive).
                    if (
                        !string.IsNullOrEmpty(search) // Only filter if there is actual search text.
                        && !b.Title.ToLower().Contains(search) // Title does NOT contain the search term.
                        && !b.Author.ToLower().Contains(search) // Author does NOT contain the search term.
                    )
                        return false; // Neither Title nor Author matched → skip this book.

                    // The book passed all active filters → include it in the result.
                    return true;
                })
                .Select(b => new // Project each Book into an anonymous object.
                {
                    // The property names become the column headers in the DataGridView.
                    // Persian names (e.g., سال_انتشار) appear as column headers because of RTL support.
                    عنوان_کتاب = b.Title, // Column: book title.
                    نویسنده_کتاب = b.Author, // Column: author name.
                    ژانر = b.Genre, // Column: genre.
                    سال_انتشار = b.PublicationYear, // Column header "سال انتشار" (Publication Year).
                    تعداد_موجود = b.CopiesAvailable, // Column header "تعداد موجود" (Available Copies).
                    هزینه_جریمه = b.LostChargePrice, // Column header "هزینه جریمه" (Lost-charge price / fine).
                })
                .ToList(); // Materialize the query into a List so it can be assigned to DataSource.

            // Bind the anonymous list to the DataGridView.
            // The grid automatically generates columns matching the anonymous type's property names.
            // Each row corresponds to one book, and the grid is read-only (set in the constructor).
            _grid.DataSource = books;
        }
    }
}
