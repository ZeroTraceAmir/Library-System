using library_system.Interfaces;
using library_system.Models;

namespace library_system.Repositories
{
    public class JsonLoanRepository : JsonRepository<Loan>, ILoanRepository 
    {
        public JsonLoanRepository(JsonDataStore store)
            : base(store, "loans.json")
        {
        }
    }
}
