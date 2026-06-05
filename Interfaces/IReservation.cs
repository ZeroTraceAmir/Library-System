using System;
using System.Collections.Generic;
using System.Text;

namespace library_system.Interfaces
{
    internal interface IReservation : IEntity
    { 
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int BookId { get; set; }
        public DateTime ReservationDate { get; set; }
        public bool IsActive { get; set; }
    }
}
