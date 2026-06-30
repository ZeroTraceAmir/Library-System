using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library_system.Models
{
    public abstract class BookNotification : Notification
    {
        public int BookId { get; set; }
    }
}
