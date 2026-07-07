using library_system.Interfaces;


namespace library_system.Models
{
    public class Book : BaseEntity, IBook
    {
        public string Title {get; set;} = string.Empty;
        public string Author {get; set;} = string.Empty;
        public int CopiesAvailable {get; set;}
        public string Genre {get;set;} = string.Empty;
        public int PublicationYear {get; set;}
        public double LostChargePrice {get; set;}
    }
}