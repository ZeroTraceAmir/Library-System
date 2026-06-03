// staff:see all books
// see all customers
// edit books
// edit profile
// search in customers
// search in books
// filter customer results
// filter book results

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library_system.Interfaces;
using library_system.Models;

namespace library_system.Services
{
    public class UserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public User? GetLoggedInUser()
        {
            List<User> users = userRepository.GetAll();
            return users.FirstOrDefault(u => u.IsLogedin);
        }

        public bool Login(string phone, string password)
        {
            List<User> users = userRepository.GetAll();
            User? user = users.FirstOrDefault(c => c.Number == phone);
            if (user == null)
            {
                return false;
            }
            if (user.Password != password)
            {
                return false;
            }
            user.IsLogedin = true;
            userRepository.Update(user);
            return true;
        }
    }
}
