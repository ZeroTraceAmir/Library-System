using System; // Provides fundamental types: String, Int32, Exception, etc. Used here for ArgumentException, StringComparison.
using System.Collections.Generic; // Provides List<T> — the return type for GetAllBooks(), SearchByTitle(), etc.
using System.Linq; // Provides LINQ extensions: .Any(), .Max(), .Where(), .ToList(), .Contains() used throughout.
using System.Threading.Tasks; // Required by the IRepository/IBookRepository interfaces (imported but not used directly in this file).
using library_system.Interfaces; // Provides IBookRepository, IRepository<Book>, IEntity — the contracts this service depends on.
using library_system.Models; // Provides the Book model class (the entity this service manages).

namespace library_system.Services // All service-layer classes live in this namespace, separating business logic from UI (Forms) and data access (Repositories).
{
    // BookService inherits from BaseService<Book>, which is:
    //   public abstract class BaseService<T>  (defined in Services/BaseService.cs)
    // BaseService<Book> provides the abstract method:
    //   protected abstract void Validate(T entity);
    // that BookService must implement. The generic T is replaced with Book at compile time.
    public class BookService : BaseService<Book>
    {
        // ── Field ──────────────────────────────────────────────────────────────
        // Holds a reference to the repository that handles actual JSON file I/O.
        // The type is the interface IBookRepository (defined in Interfaces/IBookRepository.cs),
        // which itself extends IRepository<Book> (Interfaces/IRepository.cs).
        //
        // IRepository<Book> (where T : IEntity) declares these methods:
        //   List<T> GetAll()
        //   T? GetById(int id)
        //   void Add(T entity)
        //   void Update(T entity)
        //   void Delete(int id)
        //
        // IBookRepository adds nothing extra — it exists solely so that DI/constructors
        // can inject a JsonBookRepository (Repositories/JsonBookRepository.cs) via the interface.
        //
        // The concrete type is assigned in the constructor and is typically
        // JsonBookRepository, which serializes/deserializes Data/books.json.
        private readonly IBookRepository bookRepository;

        // ── Constructor ─────────────────────────────────────────────────────────
        // Receives an IBookRepository implementation (injected manually by the caller,
        // e.g., "new BookService(new JsonBookRepository(new JsonDataStore()))" in SeeAllBooks.cs).
        // Stores it in the readonly field so all methods can delegate data access to it.
        public BookService(IBookRepository bookRepository)
        {
            // 'this.' disambiguates the field from the parameter (same name by convention).
            this.bookRepository = bookRepository;
        }

        // ── GetAllBooks ─────────────────────────────────────────────────────────
        // Returns every book stored in the JSON file as a List<Book>.
        // Delegates directly to IRepository<Book>.GetAll(), which in the concrete
        // JsonBookRepository reads Data/books.json, deserializes it, and returns the list.
        // No filtering or business logic is applied here.
        public List<Book> GetAllBooks()
        {
            return bookRepository.GetAll();
        }

        // ── GetBookById ─────────────────────────────────────────────────────────
        // Looks up a single book by its integer Id.
        // Delegates to IRepository<Book>.GetById(int id), which in JsonBookRepository
        // iterates the deserialized list and returns the first Book whose .Id matches,
        // or null if not found.
        // Returns Book? (nullable) — the caller must check for null.
        public Book? GetBookById(int id)
        {
            return bookRepository.GetById(id);
        }

        // ── AddBook (single-parameter overload) ─────────────────────────────────
        // Validates the book's fields via the abstract Validate() method (implemented below),
        // then assigns a new auto-incremented Id before persisting.
        public void AddBook(Book book)
        {
            // Calls the Validate override defined at line ~142 in this file.
            // Throws an Exception with a Persian/English message if any field is invalid.
            Validate(book);

            // Fetch the full list of existing books from the JSON store.
            List<Book> books = bookRepository.GetAll();

            // Determine the next Id:
            //   If there are any books already → Id = highest existing Id + 1.
            //   If the list is empty              → Id = 1.
            // books.Max(b => b.Id) uses LINQ to find the maximum Id value.
            // books.Any() returns false for an empty list, avoiding an InvalidOperationException from .Max().
            book.Id = books.Any() ? books.Max(b => b.Id) + 1 : 1;

            // Delegate to IRepository<Book>.Add(Book), which in JsonBookRepository
            // appends the book to the in-memory list, re-serializes the full list
            // back to Data/books.json, and writes it to disk.
            bookRepository.Add(book);
        }

        // ── AddBook (convenience overload with individual fields) ───────────────
        // Builds a new Book instance from the provided parameters, then calls the
        // primary AddBook(Book) overload above (which handles validation + Id assignment).
        // This overload exists so callers (e.g., AddBook.cs form) can pass raw values
        // without constructing a Book manually.
        public void AddBook(
            string title,          // Book.Title
            string author,         // Book.Author
            int copiesAvailable,   // Book.CopiesAvailable — number of physical/digital copies in stock.
            string genre,          // Book.Genre — e.g., "Fantasy", "Science", etc.
            int publicationYear,   // Book.PublicationYear — year the book was published.
            double lostChargePrice // Book.LostChargePrice — the fine amount charged if the book is lost.
        )
        {
            // Object-initializer syntax: creates a new Book and sets its properties,
            // then passes it to AddBook(Book) for validation + id assignment + persistence.
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

        // ── UpdateBook ──────────────────────────────────────────────────────────
        // Validates the modified book, then writes the changes back to the JSON file.
        // The book must already exist in the store (matched by its .Id).
        // Delegates to IRepository<Book>.Update(Book), which in JsonBookRepository
        // finds the book by Id in the list, replaces it, and re-serializes the file.
        public void UpdateBook(Book book)
        {
            Validate(book);
            bookRepository.Update(book);
        }

        // ── DeleteBook ──────────────────────────────────────────────────────────
        // Removes a book by its Id after verifying it exists.
        // Throws an Exception (not found) if the book does not exist in the store.
        public void DeleteBook(int id)
        {
            // Look up the book first. GetById returns null if no match is found.
            Book? book = bookRepository.GetById(id);

            // If the book was not found, throw a descriptive exception.
            // The caller (e.g., SeeAllEmployees form) should catch/handle this.
            if (book == null)
                throw new Exception("Book not found.");

            // Delegate to IRepository<Book>.Delete(int id), which in JsonBookRepository
            // removes the entry from the list and re-serializes the file.
            bookRepository.Delete(id);
        }

        // ── SearchByTitle ───────────────────────────────────────────────────────
        // Filters all books by title using a case-insensitive substring match.
        // Uses StringComparison.OrdinalIgnoreCase so "HARRY" matches "Harry Potter".
        // Returns an empty list if no books match (never returns null).
        public List<Book> SearchByTitle(string title)
        {
            return bookRepository
                .GetAll()                                             // Full list from JSON.
                .Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase)) // LINQ filter.
                .ToList();                                            // Materialise into List<Book>.
        }

        // ── SearchByAuthor ──────────────────────────────────────────────────────
        // Same logic as SearchByTitle but matches against Book.Author instead.
        public List<Book> SearchByAuthor(string author)
        {
            return bookRepository
                .GetAll()
                .Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // ── FilterByGenre ───────────────────────────────────────────────────────
        // Returns all books whose Genre matches the provided genre string exactly
        // (case-insensitive comparison using StringComparison.OrdinalIgnoreCase).
        // This is an equality check, not a substring search.
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

        // ── Validate (override from BaseService<Book>) ──────────────────────────
        // BaseService<Book> (Services/BaseService.cs) declares:
        //   protected abstract void Validate(T entity);
        // This override provides the concrete validation rules for the Book entity.
        // It is called by AddBook(Book) and UpdateBook(Book) before any write operation.
        // Throws an Exception with a descriptive message if any validation rule fails.
        protected override void Validate(Book book)
        {
            // Check that the book reference itself is not null.
            if (book == null)
                throw new Exception("Book cannot be null.");

            // Title must be a non-null, non-whitespace string.
            // string.IsNullOrWhiteSpace returns true if the string is null, empty, or only whitespace.
            if (string.IsNullOrWhiteSpace(book.Title))
                throw new Exception("Title is required.");

            // Author must be a non-null, non-whitespace string.
            if (string.IsNullOrWhiteSpace(book.Author))
                throw new Exception("Author is required.");

            // Publication year must be a positive integer.
            // Year 0 or negative makes no sense for a real book.
            if (book.PublicationYear <= 0)
                throw new Exception("Published year is invalid.");

            // Number of available copies cannot be negative.
            // Zero is allowed (out of stock), but a negative count is impossible.
            if (book.CopiesAvailable < 0)
                throw new Exception("Total copies cannot be negative.");
        }
    }
}
