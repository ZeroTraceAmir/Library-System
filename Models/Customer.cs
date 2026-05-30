using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library_system.Interfaces;
namespace library_system.Models
{
    public class Customer: ICustomer
    {
         public int Id {get; set;}
        public string Name {get; set;}
        public string Number {get; set;}
        public string HashedPassword {get; set;}
        public bool IsLogedin {get; set;}
    }
}