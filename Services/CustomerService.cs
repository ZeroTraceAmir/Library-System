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

        public void AddCustomer(Customer customer)
        {
            ValidateCustomer(customer);
            List<Customer> customers = customerRepository.GetAll();
            bool numberExist = customers.Any(c => c.Number == customer.Number);
            if (numberExist)
            {
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

        public bool Login(string phone, string password)
        {
            List<Customer> customers = customerRepository.GetAll();
            Customer? customer = customers.FirstOrDefault(c => c.Number == phone);

            if (customer == null)
                return false;

            if (customer.Password != password)
                return false;

            customer.IsLogedin = true;
            customerRepository.Update(customer);
            return true;
        }

        public void CustomerProfileEdit(string name, string phone, string password, string _phone)
        {
            List<Customer> customers = customerRepository.GetAll();
            Customer? customer = customers.FirstOrDefault(c => c.Number == _phone);
            if (customer == null)
                return;

            if (customer.Password != password)
                return;

            if (name != null)
                customer.Name = name;

            if (phone != null)
                customer.Number = phone;

            customerRepository.Update(customer);
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

        public void Logout(string phone)
        {
            List<Customer> customers = customerRepository.GetAll();
            Customer? customer = customers.FirstOrDefault(c => c.Number == phone);
            if (customer == null)
                return;

            customer.IsLogedin = false;
            customerRepository.Update(customer);
        }

        public List<Customer> GetAllCustomers()
        {
            return customerRepository.GetAll();
        }

        public List<Customer> GetFilteredCustomers(int filterIndex)
        {
            var customers = customerRepository.GetAll();

            return filterIndex switch
            {
                1 => customers.Where(c => c.HasBorrowedBook).ToList(),
                2 => customers.Where(c => c.HasReservedBook).ToList(),
                3 => customers.Where(c => c.Debt > 0).ToList(),
                _ => customers,
            };
        }

        public void DeleteCustomerAcc(string phone)
        {
            List<Customer> customers = customerRepository.GetAll();
            Customer? customer = customers.FirstOrDefault(c => c.Number == phone);
            if (customer != null)
            {
                Logout(customer.Number);
                customerRepository.Delete(customer.Id);
            }
        }
    }
}
