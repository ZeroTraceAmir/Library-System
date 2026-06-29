namespace library_system.Models
{
    public abstract class Account : Person
    {
        public string Number { get; set; }
        public string Password { get; set; }
        public bool IsLogedin { get; set; }

        public virtual string GetRoleLabel() => "کاربر";
    }
}
