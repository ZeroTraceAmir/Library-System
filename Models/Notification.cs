using System;
using System.Text.Json.Serialization;
using library_system.Enums;

namespace library_system.Models
{
    [JsonDerivedType(typeof(OverdueNotification), "OverdueNotification")]
    [JsonDerivedType(typeof(ReturnReminderNotification), "ReturnReminderNotification")]
    [JsonDerivedType(typeof(ReservationConfirmedNotification), "ReservationConfirmedNotification")]
    [JsonDerivedType(typeof(ReservationCancelledNotification), "ReservationCancelledNotification")]
    [JsonDerivedType(typeof(BookBorrowedNotification), "BookBorrowedNotification")]
    public abstract class Notification : BaseEntity
    {
        public int CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public NotificationType Type { get; set; }
        public abstract string GetMessage();
    }
}
