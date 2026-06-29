using library_system.Interfaces;
using System.Collections.Generic;
using System.Linq;
using library_system.Models;

namespace library_system.Services
{
    public class DebtService
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

        public List<Debt> GetCustomerDebts(int customerId)
        {
            return debtRepository.GetAll()
                .Where(d => d.CustomerId == customerId && !d.IsPaid)
                .ToList();
        }

        public void AddDebt(Debt debt)
        {
            List<Debt> debts = debtRepository.GetAll();
            debt.Id = debts.Count == 0 ? 1 : debts.Max(d => d.Id) + 1;
            debtRepository.Add(debt);
        }

        public void PayDebt(int debtId)
        {
            Debt? debt = debtRepository.GetById(debtId);
            if (debt == null)
                return;

            debt.IsPaid = true;
            debtRepository.Update(debt);
        }
    }
}
