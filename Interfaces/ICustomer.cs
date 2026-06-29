namespace library_system.Interfaces
{
    public interface ICustomer : IEntity
    {
        public string Name {get; set;}
        public string Number {get; set;}
        public string Password {get; set;}
        public bool IsLogedin {get; set;}
    }
}