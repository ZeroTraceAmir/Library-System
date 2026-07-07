using System;

namespace library_system.Models
{
    public class BookBorrowedNotification : BookNotification
    {
        public override string GetMessage()
        {
            return $"کتاب با موفقیت امانت گرفته شد.";
        }
    }
}
