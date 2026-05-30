using library_system.Interfaces;
using library_system.Models;

namespace library_system.Repositories
{
    public class JsonCustomerRepository : JsonRepository<Customer>, ICustomerRepository //in repository baraye type Customer ast va tamame op haye crud ro az jsonRepo be erth mibare
    {
        public JsonCustomerRepository(JsonDataStore store)
            : base(store, "customers.json")
        { // in yani constructore class pedar yani JsonRepository<Customer> ro seda mizane. be class pedar mighe: in store ro bekar bbar va data ha ro tooye file Customers.json zakhire kon
        }
    }
}