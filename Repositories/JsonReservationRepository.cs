using library_system.Interfaces;
using library_system.Models;

namespace library_system.Repositories
{
    public class JsonReservationRepository : JsonRepository<Reservation>, IReservationRepository 
    {
        public JsonReservationRepository(JsonDataStore store)
            : base(store, "reservation.json")
        {
        }
    }
}
