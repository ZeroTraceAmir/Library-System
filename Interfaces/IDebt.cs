using System;
using System.Collections.Generic;
using System.Text;

namespace library_system.Interfaces
{
    internal interface IDebt : IEntity
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public bool IsPaid { get; set; }
    }
}
