using System;

namespace library_system.Models
{
    public class ReservationCancelledNotification : BookNotification
    {
        public override string GetMessage()
        {
            return $"رزرو کتاب شما لغو شد.";
        }
    }
}
