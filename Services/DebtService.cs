using library_system.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
