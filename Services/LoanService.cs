using System;
using System.Collections.Generic;
using System.Linq;
using library_system.Interfaces;
using library_system.Models;

namespace library_system.Services
{
    public delegate void BookEventHandler(Book book, Loan? loan, int customerId);

    public class LoanService
    {
        private readonly ILoanRepository loanRepository;
        private readonly IBookRepository bookRepository;
        private readonly IDebtRepository debtRepository;
        private readonly ICustomerRepository customerRepository;
        private const decimal DailyLateFee = 5000;

        public event BookEventHandler? BookBorrowed;
        public event BookEventHandler? BookReturned;

        public LoanService(
            ILoanRepository loanRepository,
            IBookRepository bookRepository,
            IDebtRepository debtRepository,
            ICustomerRepository customerRepository)
        {
            this.loanRepository = loanRepository;
            this.bookRepository = bookRepository;
            this.debtRepository = debtRepository;
            this.customerRepository = customerRepository;
        }

        public void AddLoan(Loan loan)
        {
            List<Loan> loans = loanRepository.GetAll();

            loan.Id = loans.Count == 0 ? 1 : loans.Max(l => l.Id) + 1;
            loanRepository.Add(loan);
        }

        public List<Loan> GetAllLoans()
        {
            return loanRepository.GetAll();
        }

        public Loan? GetLoanById(int id)
        {
            return loanRepository.GetById(id);
        }

        public void UpdateLoan(Loan loan)
        {
            loanRepository.Update(loan);
        }

        public void DeleteLoan(int id)
        {
            loanRepository.Delete(id);
        }

        public List<Loan> GetLoansByCustomerId(int customerId)
        {
            return loanRepository.GetAll().Where(l => l.CustomerId == customerId).ToList();
        }

        //public void ReturnBook(int loanId, Book book)
        //{
        //    Loan? loan = loanRepository.GetById(loanId);
        //
        //    if (loan == null)
        //    {
        //        throw new Exception("امانت مورد نظر یافت نشد");
        //    }
        //
        //    loan.ReturnDate = DateTime.Now;
        //
        //    book.CopiesAvailable++;
        //
        //    loanRepository.Update(loan);
        //    bookRepository.Update(book);
        //
        //    if (loan.ReturnDate > loan.DueDate)
        //    {
        //        TimeSpan overdue = loan.ReturnDate.Value - loan.DueDate;
        //        int daysOverdue = (int)Math.Ceiling(overdue.TotalDays);
        //        decimal amount = daysOverdue * DailyLateFee;
        //
        //        List<Debt> debts = debtRepository.GetAll();
        //        Debt debt = new Debt
        //        {
        //            Id = debts.Any() ? debts.Max(d => d.Id) + 1 : 1,
        //            CustomerId = loan.CustomerId,
        //            Amount = amount,
        //            Reason = $"جریمه دیرکرد {book.Title} ({daysOverdue} روز)",
        //            IsPaid = false,
        //        };
        //        debtRepository.Add(debt);
        //
        //        Customer? customer = customerRepository.GetById(loan.CustomerId);
        //        if (customer != null)
        //        {
        //            customer.Debt += amount;
        //            customerRepository.Update(customer);
        //        }
        //    }
        //
        //    BookReturned?.Invoke(book, loan, loan.CustomerId);
        //}

        public void ReturnBook(int loanId, Book book)
        {
            Loan? loan = loanRepository.GetById(loanId);

            if (loan == null)
            {
                throw new Exception("امانت مورد نظر یافت نشد");
            }

            if (loan.ReturnDate != null)
            {
                throw new Exception("این کتاب قبلاً بازگردانده شده است");
            }

            loan.ReturnDate = DateTime.Now;

            book.CopiesAvailable++;

            loanRepository.Update(loan);
            bookRepository.Update(book);

            if (loan.ReturnDate > loan.DueDate)
            {
                TimeSpan overdue = loan.ReturnDate.Value - loan.DueDate;
                int daysOverdue = (int)Math.Ceiling(overdue.TotalDays);
                decimal amount = daysOverdue * DailyLateFee;

                List<Debt> debts = debtRepository.GetAll();
                Debt debt = new Debt
                {
                    Id = debts.Any() ? debts.Max(d => d.Id) + 1 : 1,
                    CustomerId = loan.CustomerId,
                    Amount = amount,
                    Reason = $"جریمه دیرکرد {book.Title} ({daysOverdue} روز)",
                    IsPaid = false,
                };
                debtRepository.Add(debt);

                Customer? customer = customerRepository.GetById(loan.CustomerId);
                if (customer != null)
                {
                    customer.Debt += amount;
                    customerRepository.Update(customer);
                }
            }

            BookReturned?.Invoke(book, loan, loan.CustomerId);
        }

        public void BorrowBook(Book book, int customerId)
        {
            if (book.CopiesAvailable <= 0)
                throw new Exception("کتاب در دسترس نمی باشد");

            book.CopiesAvailable--;

            Loan loan = new Loan
            {
                Id = loanRepository.GetAll().Any() ? loanRepository.GetAll().Max(l => l.Id) + 1 : 1,

                CustomerId = customerId,
                BookId = book.Id,
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddMinutes(2),
            };

            loanRepository.Add(loan);
            BookBorrowed?.Invoke(book, loan, customerId);
        }
    }
}
