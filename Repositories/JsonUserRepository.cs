using library_system.Interfaces;
using library_system.Models;

namespace library_system.Repositories
{
    public class JsonUserRepository : JsonRepository<User>, IUserRepository
    {
        public JsonUserRepository(JsonDataStore store)
            : base(store, "users.json") { }
    }
}
