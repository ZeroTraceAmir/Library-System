using System;

namespace library_system.Models
{
    public class ReservationConfirmedNotification : BookNotification
    {
        public override string GetMessage()
        {
            return $"رزرو کتاب شما تایید شد.";
        }
    }
}
