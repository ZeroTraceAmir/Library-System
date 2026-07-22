using library_system.Interfaces;

namespace library_system.Models
{
    public class Customer : Account, ICustomer
    {
        public bool HasBorrowedBook { get; set; }
        public decimal Debt { get; set; }

        public override string GetRoleLabel() => "مشتری";
    }
}