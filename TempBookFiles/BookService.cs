using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library_system.Interfaces;
using library_system.Models;
using library_system.Enums;

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
            List<Book> books = bookRepository.GetAll();
            return books;
        }

        public Book GetBookById(int id)
        {
            Book book = bookRepository.GetById(id);
            return book;
        }

        public void AddBook(Book book)
        {
            ValidateBook(book);

            List<Book> books = bookRepository.GetAll();

            bool isbnExists = books.Any(b => b.ISBN == book.ISBN);
            if (isbnExists)
            {
                throw new Exception("A book with this ISBN already exists.");
            }

            // if (book.Status == 0)
            // {
            //     book.Status = BookStatus.Available;
            // }
            book.Id = books.Any() ? books.Max(b => b.Id) + 1 :1;
            bookRepository.Add(book);
        }

        public void UpdateBook(Book book)
        {
            ValidateBook(book);

            List<Book> books = bookRepository.GetAll();

            bool isbnExists = books.Any(b => b.ISBN == book.ISBN && b.Id != book.Id);
            if (isbnExists)
            {
                throw new Exception("Another book with this ISBN already exists.");
            }

            bookRepository.Update(book);
        }

        public void DeleteBook(int id)
        {
            Book book = bookRepository.GetById(id);

            if (book == null)
            {
                throw new Exception("Book not found.");
            }

            if (book.Status == BookStatus.Loaned)
            {
                throw new Exception("Cannot delete a loaned book.");
            }

            bookRepository.Delete(id);
        }

        public List<Book> SearchByTitle(string title)
        {
            List<Book> books = bookRepository.GetAll();
            List<Book> filteredBooks = books
                .Where(b => b.Title != null && b.Title.ToLower().Contains(title.ToLower()))
                .ToList();

            return filteredBooks;
        }

        public List<Book> FilterByStatus(BookStatus status)
        {
            List<Book> books = bookRepository.GetAll();
            List<Book> filteredBooks = books
                .Where(b => b.Status == status)
                .ToList();

            return filteredBooks;
        }

        private void ValidateBook(Book book)
        {
            if (book == null)
            {
                throw new Exception("Book cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(book.Title))
            {
                throw new Exception("Title is required.");
            }

            if (string.IsNullOrWhiteSpace(book.Author))
            {
                throw new Exception("Author is required.");
            }

            if (string.IsNullOrWhiteSpace(book.ISBN))
            {
                throw new Exception("ISBN is required.");
            }

            if (book.PublicationYear <= 0)
            {
                throw new Exception("Published year is invalid.");
            }

            if (book.CopiesAvailable< 0)
            {
                throw new Exception("Total copies cannot be negative.");
            }
        } 
    }
}