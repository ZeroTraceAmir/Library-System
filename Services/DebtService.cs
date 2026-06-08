using library_system.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using library_system.Models;

namespace library_system.Services
{
    internal class DebtService
    {
        private readonly IDebtRepository debtRepository;

        public DebtService(IDebtRepository debtRepository)
        {
            this.debtRepository = debtRepository;
        }
        public List<Debt> GetAllDebts()
        {
            return debtRepository.GetAll();
        }
        
        public Debt? GetDebtById(int id)
        {
            return debtRepository.GetById(id);
        }
        public void AddDebt(Debt debt)
        {
            List<Debt> debts = debtRepository.GetAll();
            debt.Id = debts.Count == 0 ? 1 : debts.Max(d => d.Id) + 1;
            debtRepository.Add(debt);
        }

        public void UpdateDebt(Debt debt)
        {
            debtRepository.Update(debt);
        }

        public void DeleteDebt(int id)
        {
            debtRepository.Delete(id);
        }

        public void IncreaseDebt(int debtId, decimal amount)
        {
            Debt? debt = debtRepository.GetById(debtId);

            if (debt == null)
            {
                return;
            }

            debt.Amount += amount;

            debtRepository.Update(debt);
        }
        public void SetDebtToZero(int debtId)
        {
            Debt? debt = debtRepository.GetById(debtId);
            
            if (debt != null)
            {
                debt.Amount = 0;
                debt.IsPaid = true;
                debtRepository.Update(debt);
            }
        }

        public List<Debt> GetCustomerDebts(int customerId)
        {
            return debtRepository.GetAll()
            .Where(d => d.CustomerId == customerId)
                .ToList();
        }

        public void PayDebt(int debtId)
        {
            Debt? debt = debtRepository.GetById(debtId);

            if (debt == null)
            {
                return;
            }

            debt.IsPaid = true;

            debtRepository.Update(debt);
        }

        public void LoseBook(Book book, int customerId)
        {
            Debt debt = new Debt
            {
                Id = debtRepository.GetAll().Any()?
                debtRepository.GetAll().Max(d => d.Id) + 1 : 1,

                CustomerId = customerId,
                Amount = (decimal)book.LostChargePrice,
                Reason = "گمشده است",
                IsPaid = false
            };

            debtRepository.Add(debt);
        }
    }
}
