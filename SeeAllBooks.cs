using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using library_system.Services;

namespace library_system
{
    public class SeeAllBooks : Form
    {
        private readonly BookService _bookService;
        private readonly DataGridView _grid;
        private readonly ComboBox _cmbFilter;
        private readonly TextBox _txtSearch;

        public SeeAllBooks()
        {
            var store = new Repositories.JsonDataStore();
            _bookService = new BookService(new Repositories.JsonBookRepository(store));

            this.Text = "دیدن همه کتاب ها";
            this.WindowState = FormWindowState.Maximized;

            var topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 50,
                Padding = new Padding(10),
            };

            _txtSearch = new TextBox
            {
                Font = new Font("Tahoma", 11F),
                Width = 200,
                PlaceholderText = "جستجو...",
            };
            _txtSearch.TextChanged += (s, e) => RefreshGrid();

            _cmbFilter = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Tahoma", 11F),
                Width = 150,
            };
            _cmbFilter.SelectedIndexChanged += (s, e) => RefreshGrid();

            topPanel.Controls.Add(_cmbFilter);
            topPanel.Controls.Add(_txtSearch);

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                Font = new Font("Tahoma", 10F),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RightToLeft = RightToLeft.Yes,
            };

            Controls.Add(_grid);
            Controls.Add(topPanel);
            Controls.Add(new Button
            {
                Text = "بازگشت",
                Dock = DockStyle.Bottom,
                Height = 50,
                DialogResult = DialogResult.Cancel,
            });

            LoadGenres();
            RefreshGrid();
        }

        private void LoadGenres()
        {
            _cmbFilter.Items.Add("همه");

            var genres = _bookService.GetAllBooks()
                .Select(b => b.Genre)
                .Where(g => !string.IsNullOrWhiteSpace(g))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(g => g)
                .ToList();

            foreach (var genre in genres)
                _cmbFilter.Items.Add(genre);

            _cmbFilter.SelectedIndex = 0;
        }

        private void RefreshGrid()
        {
            var search = _txtSearch.Text.Trim().ToLower();

            var books = _bookService.GetAllBooks()
                .Where(b =>
                {
                    if (_cmbFilter.SelectedIndex > 0)
                    {
                        var selectedGenre = _cmbFilter.SelectedItem.ToString();
                        if (!b.Genre.Equals(selectedGenre, StringComparison.OrdinalIgnoreCase))
                            return false;
                    }

                    if (!string.IsNullOrEmpty(search) &&
                        !b.Title.ToLower().Contains(search) &&
                        !b.Author.ToLower().Contains(search))
                        return false;

                    return true;
                })
                .Select(b => new
                {
                    b.Title,
                    b.Author,
                    b.Genre,
                    سال_انتشار = b.PublicationYear,
                    تعداد_موجود = b.CopiesAvailable,
                    هزینه_جریمه = b.LostChargePrice,
                })
                .ToList();

            _grid.DataSource = books;
        }
    }
}
