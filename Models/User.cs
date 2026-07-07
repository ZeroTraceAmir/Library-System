using library_system.Enums;
using library_system.Interfaces;

namespace library_system.Models
{
    public class User : Account, IUser
    {
        public UserStatus Role { get; set; }

        public override string GetRoleLabel() => Role == UserStatus.admin ? "مدیر" : "کارمند";
    }
}
