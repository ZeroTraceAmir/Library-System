// namespace library_system;
// using library_system.Interfaces;
// using library_system.Repositories;
// using library_system.Services;

// static class Program
// {
//     [STAThread]
//     static void Main()
//     {
//         ApplicationConfiguration.Initialize();

//         JsonDataStore dataStore = new JsonDataStore();
//         ICustomerRepository customerRepository = new JsonCustomerRepository(dataStore);
//         CustomerService customerService = new CustomerService(customerRepository);

//         Application.Run(new RegisterForm(customerService));
//     }
// }

// using System;
// using System.Windows.Forms;

// namespace library_system
// {
//     internal static class Program
//     {
//         [STAThread]
//         static void Main()
//         {
//             Application.EnableVisualStyles();
//             Application.SetCompatibleTextRenderingDefault(false);
//             Application.Run(new Form1());
//         }
//     }
// }
using System;
using System.Windows.Forms;
using library_system.Enums;
using library_system.Models;
using library_system.Repositories;
using library_system.Services;

namespace library_system
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // build the dependency chain just for the login check
            var store = new JsonDataStore();
            var customerRepository = new JsonCustomerRepository(store);
            var customerService = new CustomerService(customerRepository);
            var userRepository = new JsonUserRepository(store);
            var userService = new UserService(userRepository);

            Customer? CustomerLoggedIn = customerService.GetLoggedInCustomer();
            User? UserLoggedIn = userService.GetLoggedInUser();
            // if (loggedIn != null)
            //     Application.Run(new Home());
            // else
            //     Application.Run(new Form1());
            if (UserLoggedIn != null)
            {
                Form form = UserLoggedIn.Role == Enums.UserStatus.admin
                    ? new AdminPanel(UserLoggedIn)
                    : new StaffPanel(UserLoggedIn);
                Application.Run(form);
            }
            else if (CustomerLoggedIn != null)
            {
                Application.Run(new Home(CustomerLoggedIn));
            }
            else
            {
                Application.Run(new Form1());
            }
        }
    }
}
