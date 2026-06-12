using library_system.Interfaces;
using System;

namespace library_system.Models
{
    public class Reservation : IEntity 
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int BookId { get; set; }
        public DateTime ReservationDate { get; set; } 
        public bool IsActive { get; set; }
    }
}
