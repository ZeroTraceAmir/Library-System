using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library_system.Interfaces;
using library_system.Models;

namespace library_system.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            this.customerRepository = customerRepository;
        }


        public void AddCustomer(Customer customer){
            ValidateCustomer(customer);
            List<Customer> customers = customerRepository.GetAll();
            bool numberExist = customers.Any(c => c.Number == customer.Number);
            if (numberExist){
                throw new Exception("این شماره از قبل ثبت نام کرده است");
            }
            customer.Id = customers.Count == 0 ? 1 : customers.Max(c => c.Id) + 1;
            customerRepository.Add(customer);
        }
        public Customer? GetLoggedInCustomer()
{
    List<Customer> customers = customerRepository.GetAll();
    return customers.FirstOrDefault(c => c.IsLogedin);


}


        public void Login(Customer customer){
            List<Customer> customers = customerRepository.GetAll();
           bool numberDoesNotExists = customers.Any(c.Number != customer.Number);
           if (numberDoesNotExists)
           {
                throw new Exception("این شماره تا کنون در کتاب خانه ثبت نام تکرده است");
           } else if (!numberDoesNotExists && customers.)
           {
            
           }
        }

        private void ValidateCustomer(Customer customer)
        {
            if (customer == null)
            {
                throw new Exception("عضو نمیتواند دوچ باشد");
            }

            if (string.IsNullOrWhiteSpace(customer.Name))
            {
                throw new Exception("وارد کردن نام اجباری است");
            }

            if (string.IsNullOrWhiteSpace(customer.Number))
            {
                throw new Exception("وارد کردن شماره تماس،‌اجباری است");
            }
        }

    }
}