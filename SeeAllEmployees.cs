using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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
            var store = new Repositories.JsonDataStore();
            _userService = new UserService(new Repositories.JsonUserRepository(store));

            this.Text = "دیدن همه کارمندان کتابخانه";
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
                Items = { "همه", "ادمین", "کارمند" },
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

            var loggedInUser = _userService.GetLoggedInUser();
            if (loggedInUser != null && loggedInUser.Role == Enums.UserStatus.admin)
            {
                _deleteColumn = new DataGridViewButtonColumn
                {
                    HeaderText = "حذف",
                    Text = "حذف",
                    UseColumnTextForButtonValue = true,
                    Width = 80,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                };
                _grid.Columns.Add(_deleteColumn);
                _grid.CellClick += OnGridCellClick;
            }

            Controls.Add(_grid);
            Controls.Add(topPanel);
            Controls.Add(new Button
            {
                Text = "بازگشت",
                Dock = DockStyle.Bottom,
                Height = 50,
                DialogResult = DialogResult.Cancel,
            });

            RefreshGrid();
        }

        private void OnGridCellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex != _deleteColumn.Index)
                return;

            var row = _grid.Rows[e.RowIndex];
            var userId = Convert.ToInt32(row.Cells["Id"].Value);
            var targetRole = Convert.ToInt32(row.Cells["Role"].Value);

            var loggedInUser = _userService.GetLoggedInUser();
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

            var result = MessageBox.Show(
                "آیا از حذف این کارمند اطمینان دارید؟",
                "تأیید حذف",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            _userService.DeleteUser(userId);
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            var search = _txtSearch.Text.Trim().ToLower();

            var users = _userService.GetFilteredUsers(_cmbFilter.SelectedIndex)
                .Where(u => string.IsNullOrEmpty(search) ||
                    u.Name.ToLower().Contains(search) ||
                    u.Number.ToLower().Contains(search))
                .Select(u => new
                {
                    Id = u.Id,
                    u.Name,
                    شماره = u.Number,
                    نقش = u.Role == Enums.UserStatus.admin ? "ادمین" : "کارمند",
                    Role = (int)u.Role
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
