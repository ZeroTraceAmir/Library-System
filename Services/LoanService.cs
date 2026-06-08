using library_system.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using library_system.Models;

namespace library_system.Services
{
    internal class LoanService
    {
        private readonly ILoanRepository loanRepository;

        public LoanService(ILoanRepository loanRepository)
        {
            this.loanRepository = loanRepository;
        }
        public void AddLoan(Loan loan)
        {
            List<Loan> loans = loanRepository.GetAll();

            loan.Id = loans.Count == 0 ? 1 :
                loans.Max(l => l.Id) + 1;
            loanRepository.Add(loan);
        }
        public List<Loan> GetAllLoans()
        {
            return loanRepository.GetAll();
        }
        public Loan? GetLoanById(int id )
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
            return loanRepository.GetAll()
            .Where(l => l.CustomerId == customerId)
                .ToList();
        }

        public void ReturnBook(int loanId, Book book )
        {
            Loan? loan = loanRepository.GetById(loanId);

            if (loan == null)
            {
                throw new Exception("امانت مورد نظر یافت نشد")ک
            }

            loan.ReturnDate = DateTime.Now;

            book.CopiesAvailable++;

            loanRepository.Update(loan);
        }

        public void BorrowBook(Book book, int customerId)
        {
            if (book.CopiesAvailable <= 0)
                throw new Exception("کتاب در دسترس نمی باشد");

            book.CopiesAvailable--;

            Loan loan = new Loan
            {
                Id = loanRepository.GetAll().Any() ?
                loanRepository.GetAll().Max(l => l.Id) + 1 : 1,

                CustomerId = customerId,
                BookId = book.Id,
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14)
            };

            loanRepository.Add(loan);
        }
    }
}
