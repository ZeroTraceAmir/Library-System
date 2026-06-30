using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library_system.Models
{
    public class ReturnReminderNotification : LoanNotification
    {
        public override string GetMessage()
        {
            return $"مهلت بازگرداندن کتاب نزدیک است.";
        }
    }
}
