using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using library_system.Enums;
using library_system.Services;

namespace library_system
{
    public class SeeAllCustomers : Form
    {
        private readonly CustomerService _customerService;
        private readonly DataGridView _grid;
        private readonly ComboBox _cmbFilter;
        private readonly TextBox _txtSearch;

        public SeeAllCustomers()
        {
            Repositories.JsonDataStore store = new Repositories.JsonDataStore();
            _customerService = new CustomerService(new Repositories.JsonCustomerRepository(store));

            this.Text = "دیدن همه مشتریان کتابخانه";
            this.WindowState = FormWindowState.Maximized;

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
            _txtSearch.TextChanged += (s, e) => RefreshGrid();

            _cmbFilter = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Vazir", 11F),
                Width = 150,
                Items = { "همه", "کتاب قرض گرفته", "کتاب رزرو کرده", "بدهکار" },
                SelectedIndex = 0,
                BackColor = ColorTranslator.FromHtml("#252836"),
                ForeColor = Color.WhiteSmoke,
            };
            _cmbFilter.SelectedIndexChanged += (s, e) => RefreshGrid();

            topPanel.Controls.Add(_cmbFilter);
            topPanel.Controls.Add(_txtSearch);

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                Font = new Font("Vazir", 10F),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RightToLeft = RightToLeft.Yes,
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
            Controls.Add(_grid);
            Controls.Add(topPanel);
            Controls.Add(
                new Button
                {
                    Text = "بازگشت",
                    Dock = DockStyle.Bottom,
                    Height = 50,
                    DialogResult = DialogResult.Cancel,
                    BackColor = ColorTranslator.FromHtml("#00ff9c"),
                    ForeColor = ColorTranslator.FromHtml("#111520"),
                    Font = new Font("Vazir", 11F, FontStyle.Bold),
                }
            );

            RefreshGrid();
        }

        private void RefreshGrid()
        {
            string search = _txtSearch.Text.Trim().ToLower();

            var customers = _customerService
                .GetFilteredCustomers((CustomerFilter)_cmbFilter.SelectedIndex)
                .Where(c =>
                    string.IsNullOrEmpty(search)
                    || c.Name.ToLower().Contains(search)
                    || c.Number.ToLower().Contains(search)
                )
                .Select(c => new { نام = c.Name, شماره = c.Number })
                .ToList();

            _grid.DataSource = customers;
        }
    }
}
