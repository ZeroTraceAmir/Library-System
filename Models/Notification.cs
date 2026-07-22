using System;
using System.Text.Json.Serialization;
using library_system.Enums;

namespace library_system.Models
{
    [JsonDerivedType(typeof(OverdueNotification), "OverdueNotification")]
    [JsonDerivedType(typeof(ReturnReminderNotification), "ReturnReminderNotification")]
    [JsonDerivedType(typeof(BookBorrowedNotification), "BookBorrowedNotification")]
    // inja darim be in kelas ro be  JsonSerializer  moarefi mikonim to heyne khondan file .json bedanad in notification az noe Oberdue/return/borroww hast.
    public abstract class Notification : BaseEntity

    {
        public int CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public NotificationType Type { get; set; }
        public abstract string GetMessage();
    // inja abstract hast ta be bachehash begim ke agha, man nemidonam payam chiye, vali shoma movazaf hastid ke bedonind va moshakhas konid ke payam chiye
    }
}
