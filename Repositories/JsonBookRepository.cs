using library_system.Interfaces;
using library_system.Models;

namespace library_system.Repositories
{
    public class JsonBookRepository : JsonRepository<Book>, IBookRepository
    {
        public JsonBookRepository(JsonDataStore store)
            : base(store, "books.json")
        {
        }
    }
}
