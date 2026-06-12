using library_system.Interfaces;
using library_system.Models;

namespace library_system.Repositories
{
    public class JsonDebtRepository : JsonRepository<Debt>, IDebtRepository 
    {
        public JsonDebtRepository(JsonDataStore store)
            : base(store, "debts.json")
        {
        }
    }
}
