using library_system.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

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

            loan.Id = loan.Count == 0 ? 1 :
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
        

    }
}
