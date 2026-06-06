using library_system.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace library_system.Repositories
{
    internal class JsonDebtRepository : JsonDebtRepository<Debt>, IDebtRepository 
    {
        public JsonDabtRepository(JsonDataStore store)
            : base(store, "debts.json")
        {
        }
    }
}
