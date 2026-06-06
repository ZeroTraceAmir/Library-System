using library_system.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace library_system.Repositories
{
    internal class JsonLoanRepository : JsonRepository<Loan>, ILoanRepository 
    {
        public JsonLaonRepository(JsonDataStore store)
            : base(store, "loan.json")
        {
        }
    }
}
