using System;

namespace library_system.Interfaces
{
    public interface IDebt : IEntity
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public bool IsPaid { get; set; }
    }
}
