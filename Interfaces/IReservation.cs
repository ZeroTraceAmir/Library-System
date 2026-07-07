using System;

namespace library_system.Interfaces
{
    public interface IReservation : IEntity
    { 
        public int CustomerId { get; set; }
        public int BookId { get; set; }
        public DateTime ReservationDate { get; set; }
        public bool IsActive { get; set; }
    }
}
