using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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
            var store = new Repositories.JsonDataStore();
            _customerService = new CustomerService(new Repositories.JsonCustomerRepository(store));

            this.Text = "دیدن همه مشتریان کتابخانه";
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
                Items = { "همه", "کتاب قرض گرفته", "کتاب رزرو کرده", "بدهکار" },
                SelectedIndex = 0,
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
            Controls.Add(
                new Button
                {
                    Text = "بازگشت",
                    Dock = DockStyle.Bottom,
                    Height = 50,
                    DialogResult = DialogResult.Cancel,
                }
            );

            RefreshGrid();
        }

        private void RefreshGrid()
        {
            var search = _txtSearch.Text.Trim().ToLower();

            var customers = _customerService.GetFilteredCustomers(_cmbFilter.SelectedIndex)
                .Where(c => string.IsNullOrEmpty(search) ||
                    c.Name.ToLower().Contains(search) ||
                    c.Number.ToLower().Contains(search))
                .Select(c => new { c.Name, شماره = c.Number })
                .ToList();

            _grid.DataSource = customers;
        }
    }
}
