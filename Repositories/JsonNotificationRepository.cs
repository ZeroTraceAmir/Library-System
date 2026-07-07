using library_system.Interfaces;
using library_system.Models;

namespace library_system.Repositories
{
    public class JsonNotificationRepository : JsonRepository<Notification>, INotificationRepository
    {
        public JsonNotificationRepository(JsonDataStore store)
            : base(store, "notifications.json")
        {
        }
    }
}
