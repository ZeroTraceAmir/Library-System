using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library_system.Enums;
using library_system.Interfaces;

namespace library_system.Models
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Password { get; set; }
        public UserStatus Role { get; set; }
        public bool IsLogedin { get; set; }
    }
}
