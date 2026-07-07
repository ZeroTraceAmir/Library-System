using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library_system.Models
{
    public class OverdueNotification : LoanNotification
    {
        public override string GetMessage()
        {
            return $"کتاب شما دیر شده است.";
        }
    }
}
