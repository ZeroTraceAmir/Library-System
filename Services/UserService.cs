// staff:see all books
// see all users
// edit books
// edit profile
// search in users
// search in books
// filter user results
// filter book results

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library_system.Enums;
using library_system.Interfaces;
using library_system.Models;

namespace library_system.Services
{
    public class UserService : BaseService<User>
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

        public void UserProfileEdit(string name, string phone, string password, string _phone)
        {
            List<User> users = userRepository.GetAll();
            User? user = users.FirstOrDefault(c => c.Number == _phone);
            if (user == null)
                return;

            if (user.Password != password)
                return;

            if (name != "")
                user.Name = name;

            if (phone != "")
                user.Number = phone;

            userRepository.Update(user);
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

        public void Logout(string phone)
        {
            List<User> users = userRepository.GetAll();
            User? user = users.FirstOrDefault(c => c.Number == phone);
            if (user == null)
                return;

            user.IsLogedin = false;
            userRepository.Update(user);
        }

        public List<User> GetAllUsers()
        {
            return userRepository.GetAll();
        }

        public List<User> GetFilteredUsers(UserFilter filter)
        {
            List<User> users = userRepository.GetAll();

            return filter switch
            {
                UserFilter.Admins => users.Where(u => u.Role == UserStatus.admin).ToList(),
                UserFilter.Staff => users.Where(u => u.Role == UserStatus.staff).ToList(),
                _ => users,
            };
        }

        public List<User> this[string searchTerm]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return GetAllUsers();

                return GetAllUsers()
                    .Where(u =>
                        u.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                        || u.Number.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        public void DeleteUser(int id)
        {
            userRepository.Delete(id);
        }

        public void AddEmployee(
            string name,
            string phone,
            string password,
            string repeatPassword,
            UserStatus role
        )
        {
            if (password != repeatPassword)
                throw new Exception("رمز عبور و تکرار آن یکسان نیستند");

            if (password.Length < 4)
                throw new Exception("رمز عبور باید حداقل ۴ کاراکتر باشد");

            User user = new User
            {
                Name = name,
                Number = phone,
                Password = password,
                Role = role,
                IsLogedin = false,
            };

            Validate(user);
            List<User> users = userRepository.GetAll();
            bool numberExist = users.Any(c => c.Number == user.Number);
            if (numberExist)
                throw new Exception("این شماره از قبل ثبت نام کرده است");

            user.Id = users.Count == 0 ? 1 : users.Max(c => c.Id) + 1;
            userRepository.Add(user);
        }

        protected override void Validate(User user)
        {
            if (user == null)
            {
                throw new Exception("عضو نمیتواند دوچ باشد");
            }

            if (string.IsNullOrWhiteSpace(user.Name))
            {
                throw new Exception("وارد کردن نام اجباری است");
            }

            if (string.IsNullOrWhiteSpace(user.Number))
            {
                throw new Exception("وارد کردن شماره تماس،‌اجباری است");
            }
        }
    }
}
