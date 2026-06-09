using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library_system.Interfaces;
using library_system.Models;

namespace library_system.Services
{
    public class BookService
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
            ValidateBook(book);

            List<Book> books = bookRepository.GetAll();
            book.Id = books.Any() ? books.Max(b => b.Id) + 1 : 1;
            bookRepository.Add(book);
        }

        public void UpdateBook(Book book)
        {
            ValidateBook(book);
            bookRepository.Update(book);
        }

        public void DeleteBook(int id)
        {
            Book? book = bookRepository.GetById(id);
            if (book == null)
                throw new Exception("Book not found.");

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

        private void ValidateBook(Book book)
        {
            if (book == null)
                throw new Exception("Book cannot be null.");

            if (string.IsNullOrWhiteSpace(book.Title))
                throw new Exception("Title is required.");

            if (string.IsNullOrWhiteSpace(book.Author))
                throw new Exception("Author is required.");

            if (book.PublicationYear <= 0)
                throw new Exception("Published year is invalid.");

            if (book.CopiesAvailable < 0)
                throw new Exception("Total copies cannot be negative.");
        }
    }
}
