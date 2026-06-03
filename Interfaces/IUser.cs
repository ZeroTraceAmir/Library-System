using library_system.Enums;

namespace library_system.Interfaces
{
    public interface IUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public UserStatus Role { get; set; }
        public bool IsLogedin { get; set; }
    }
}
