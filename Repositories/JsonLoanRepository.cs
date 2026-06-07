using library_system.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using library_system.Models;

namespace library_system.Repositories
{
    internal class JsonLoanRepository : JsonRepository<Loan>, ILoanRepository 
    {
        public JsonLoanRepository(JsonDataStore store)
            : base(store, "loans.json")
        {
        }
    }
}
