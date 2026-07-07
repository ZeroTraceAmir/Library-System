using System;
using System.Drawing;
using System.Windows.Forms;
using library_system.Models;
using library_system.Services;

namespace library_system
{
    public class AddBook : Form
    {
        private readonly BookService _bookService;

        public AddBook()
        {
            Repositories.JsonDataStore store = new Repositories.JsonDataStore();
            _bookService = new BookService(new Repositories.JsonBookRepository(store));

            this.Text = "اضافه کردن کتاب";
            this.WindowState = FormWindowState.Maximized;

            TableLayoutPanel table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 7,
                Padding = new Padding(80, 40, 80, 0),
                AutoSize = true,
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

            string[] labels = { "عنوان:", "نویسنده:", "ژانر:", "سال انتشار:", "تعداد موجود:", "هزینه جریمه:" };
            TextBox txtTitle = new TextBox();
            TextBox txtAuthor = new TextBox();
            TextBox txtGenre = new TextBox();
            TextBox txtYear = new TextBox();
            TextBox txtCopies = new TextBox();
            TextBox txtLostCharge = new TextBox();
            Control[] inputs = { txtTitle, txtAuthor, txtGenre, txtYear, txtCopies, txtLostCharge };

            for (int i = 0; i < 6; i++)
            {
                table.Controls.Add(new Label
                {
                    Text = labels[i],
                    Font = new Font("Tahoma", 11F),
                    TextAlign = ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill,
                }, 0, i);

                inputs[i].Font = new Font("Tahoma", 11F);
                inputs[i].Dock = DockStyle.Fill;
                inputs[i].Padding = new Padding(5);
                table.Controls.Add(inputs[i], 1, i);
            }

            Button btnAdd = new Button
            {
                Text = "اضافه کردن",
                Font = new Font("Tahoma", 11F),
                Height = 40,
                AutoSize = true,
            };
            btnAdd.Click += (s, e) =>
            {
                try
                {
                    string title = txtTitle.Text.Trim();
                    string author = txtAuthor.Text.Trim();
                    string genre = txtGenre.Text.Trim();
                    int year = int.Parse(txtYear.Text.Trim());
                    int copies = int.Parse(txtCopies.Text.Trim());
                    double lostCharge = double.Parse(txtLostCharge.Text.Trim());

                    Book book = new Book
                    {
                        Title = title,
                        Author = author,
                        Genre = genre,
                        PublicationYear = year,
                        CopiesAvailable = copies,
                        LostChargePrice = lostCharge,
                    };

                    _bookService.AddBook(book);

                    MessageBox.Show(this, "کتاب با موفقیت اضافه شد", "موفق",
                        MessageBoxButtons.OK, MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                    Close();
                }
                catch (FormatException)
                {
                    MessageBox.Show(this, "لطفاً مقادیر عددی را به درستی وارد کنید", "خطا",
                        MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "خطا",
                        MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                }
            };
            table.Controls.Add(btnAdd, 1, 6);

            Controls.Add(table);

            Controls.Add(new Button
            {
                Text = "بازگشت",
                Dock = DockStyle.Bottom,
                Height = 50,
                DialogResult = DialogResult.Cancel,
            });
        }
    }
}
