using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library_system.Interfaces
{
    public interface ICustomer : IEntity
    {
        public int Id {get; set;}
        public string Name {get; set;}
        public string Number {get; set;}
        public string Password {get; set;}
        public bool IsLogedin {get; set;}
    }
}