using System;
using System.Collections.Generic;
using System.Text;

namespace library_system.Interfaces
{
    internal interface ILoan : IEntity
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int BookId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
