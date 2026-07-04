using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using library_system.Enums;
using library_system.Models;
using library_system.Services;

namespace library_system
{
    public class SeeAllEmployees : Form
    {
        private readonly UserService _userService;
        private readonly DataGridView _grid;
        private readonly ComboBox _cmbFilter;
        private readonly TextBox _txtSearch;
        private readonly DataGridViewButtonColumn _deleteColumn;

        public SeeAllEmployees()
        {
            Repositories.JsonDataStore store = new Repositories.JsonDataStore();
            _userService = new UserService(new Repositories.JsonUserRepository(store));

            this.Text = "دیدن همه کارمندان کتابخانه";
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
                Items = { "همه", "ادمین", "کارمند" },
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

            _grid.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#1f293d");
            _grid.DefaultCellStyle.ForeColor = Color.White;
            _grid.DefaultCellStyle.Font = new Font("Vazir", 9F);
            _grid.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#00ff9c");
            _grid.DefaultCellStyle.SelectionForeColor = ColorTranslator.FromHtml("#111520");

            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#00ff9c");
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#111520");
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Vazir", 9F, FontStyle.Bold);
            _grid.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            User? loggedInUser = _userService.GetLoggedInUser();
            if (loggedInUser != null && loggedInUser.Role == Enums.UserStatus.admin)
            {
                _deleteColumn = new DataGridViewButtonColumn
                {
                    HeaderText = "حذف",
                    Text = "حذف",
                    UseColumnTextForButtonValue = true,
                    Width = 80,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    FlatStyle = FlatStyle.Flat,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = ColorTranslator.FromHtml("#e63946"), // A nice warning red for delete, or use #00ff9c for your green
                        ForeColor = Color.White, // Text color inside the button
                        SelectionBackColor = ColorTranslator.FromHtml("#e63946"), // Keeps the color same when row is selected
                        SelectionForeColor = Color.White,
                        Font = new Font("Vazir", 13F, FontStyle.Bold),
                    },
                };
                _grid.Columns.Add(_deleteColumn);
                _grid.CellClick += OnGridCellClick;
            }

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

        private void OnGridCellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex != _deleteColumn.Index)
                return;

            DataGridViewRow row = _grid.Rows[e.RowIndex];
            int userId = Convert.ToInt32(row.Cells["Id"].Value);
            int targetRole = Convert.ToInt32(row.Cells["Role"].Value);

            User? loggedInUser = _userService.GetLoggedInUser();
            if (loggedInUser == null)
                return;

            if (userId == loggedInUser.Id)
            {
                MessageBox.Show("شما نمی‌توانید خودتان را حذف کنید");
                return;
            }

            if (targetRole == (int)Enums.UserStatus.admin)
            {
                MessageBox.Show("شما نمی‌توانید ادمین را حذف کنید");
                return;
            }

            DialogResult result = MessageBox.Show(
                "آیا از حذف این کارمند اطمینان دارید؟",
                "تأیید حذف",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            _userService.DeleteUser(userId);
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            string search = _txtSearch.Text.Trim().ToLower();

            var users = _userService
                .GetFilteredUsers((UserFilter)_cmbFilter.SelectedIndex)
                .Where(u =>
                    string.IsNullOrEmpty(search)
                    || u.Name.ToLower().Contains(search)
                    || u.Number.ToLower().Contains(search)
                )
                .Select(u => new
                {
                    Id = u.Id,
                    نام = u.Name,
                    شماره = u.Number,
                    نقش = u.Role == Enums.UserStatus.admin ? "ادمین" : "کارمند",
                    Role = (int)u.Role,
                })
                .ToList();

            _grid.DataSource = users;
            if (_grid.Columns.Contains("Id"))
                _grid.Columns["Id"].Visible = false;
            if (_grid.Columns.Contains("Role"))
                _grid.Columns["Role"].Visible = false;
        }
    }
}
