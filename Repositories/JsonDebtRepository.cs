using library_system.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using library_system.Models;

namespace library_system.Repositories
{
    internal class JsonDebtRepository : JsonRepository<Debt>, IDebtRepository 
    {
        public JsonDebtRepository(JsonDataStore store)
            : base(store, "debts.json")
        {
        }
    }
}
