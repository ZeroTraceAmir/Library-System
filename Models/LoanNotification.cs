using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library_system.Models
{
    public abstract class LoanNotification : BookNotification
    {
        public DateTime DueDate { get; set; }
    }
}
