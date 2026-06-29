using library_system.Interfaces;
using System;

namespace library_system.Models
{
    public class Debt : BaseEntity, IDebt
    {
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public bool IsPaid { get; set; }
    }
}
