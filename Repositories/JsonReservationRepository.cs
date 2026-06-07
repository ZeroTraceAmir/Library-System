using library_system.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using library_system.Models;

namespace library_system.Repositories
{
    internal class JsonReservationRepository : JsonRepository<Reservation>, IReservationRepository 
    {
        public JsonReservationRepository(JsonDataStore store)
            : base(store, "reservation.json")
        {
        }
    }
}
