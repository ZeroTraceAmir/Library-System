namespace library_system.Interfaces
{
    public interface IBook : IEntity
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int CopiesAvailable { get; set; }
        public string Genre { get; set; }
        public int PublicationYear { get; set; }
        public double LostChargePrice { get; set; }
    }
}
