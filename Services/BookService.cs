using library_system.Interfaces;
using library_system.Models; 

namespace library_system.Services 
{
    public class BookService : BaseService<Book>
    {
        private readonly IBookRepository bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            this.bookRepository = bookRepository;
        }

        public List<Book> GetAllBooks()
        {
            return bookRepository.GetAll();
        }

        public Book? GetBookById(int id)
        {
            return bookRepository.GetById(id);
        }

        public void AddBook(Book book)
        {
            Validate(book);
            List<Book> books = bookRepository.GetAll();
            book.Id = books.Any() ? books.Max(b => b.Id) + 1 : 1;
            bookRepository.Add(book);
        }

        //baraye addBook.cs
        public void AddBook(
            string title,          
            string author,         
            int copiesAvailable,   
            string genre,          
            int publicationYear,   
            double lostChargePrice 
        )
        {
            AddBook(
                new Book
                {
                    Title = title,
                    Author = author,
                    CopiesAvailable = copiesAvailable,
                    Genre = genre,
                    PublicationYear = publicationYear,
                    LostChargePrice = lostChargePrice,
                }
            );
        }

        public void UpdateBook(Book book)
        {
            Validate(book);
            bookRepository.Update(book);
        }

        public void DeleteBook(int id)
        {
            Book? book = bookRepository.GetById(id);

            if (book == null)
                throw new Exception("متاسفانه کتاب پیدا نشد");

            bookRepository.Delete(id);
        }

        public List<Book> SearchByTitle(string title)
        {
            return bookRepository
                .GetAll()                                             
                .Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase)) 
                .ToList();                                            
        }

        public List<Book> SearchByAuthor(string author)
        {
            return bookRepository
                .GetAll()
                .Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Book> FilterByGenre(string genre)
        {
            return bookRepository
                .GetAll()
                .Where(b => b.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Book> this[string searchTerm]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return GetAllBooks();

                return GetAllBooks()
                    .Where(b =>
                        b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                        || b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        protected override void Validate(Book book)
        {
            if (book == null)
                throw new Exception("کتاب نمیتواند پوچ باشد");

            if (string.IsNullOrWhiteSpace(book.Title))
                throw new Exception("عنوان کتاب نمیتواند خالی باشد");

            if (string.IsNullOrWhiteSpace(book.Author))
                throw new Exception("نویسنده کتاب نمیتواند خالی باشد");

            if (book.PublicationYear <= 0)
                throw new Exception("سال وارد شده به عنوان سال انتشار کتاب، معتبر نیست");

            if (book.CopiesAvailable < 0)
                throw new Exception("تعداد نسخه های موجود نمیتواند کمتر از صفر باشد");
        }
    }
}
