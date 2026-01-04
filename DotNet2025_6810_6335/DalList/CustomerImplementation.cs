using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using static Dal.DataSource;

namespace Dal;


internal class CustomerImplementation : ICustomer
{
    // יצירת לקוח חדש
    public int Create(Customer item)
    {
        // בדיקה אם כבר קיים לקוח עם אותו מזהה (למשל תעודת זהות)
        if (Read(item.IDNumber) != null)
            throw new Exception($"Customer with ID {item.IDNumber} already exists.");

        Customers.Add(item);
        return item.IDNumber;
    }

    // CustomerImplementation: CRUD for Customer over DataSource

    public Customer? Read(int id) =>
        Customers.FirstOrDefault(c => c?.IDNumber == id);

    public List<Customer> ReadAll() =>
        new List<Customer>(Customers!);

    public void Update(Customer item)
    {
        Customer? existing = Read(item.IDNumber);
        if (existing == null)
            throw new Exception($"Customer with ID {item.IDNumber} does not exist.");

        Customers.Remove(existing);
        Customers.Add(item);
    }

    public void Delete(int id)
    {
        Customer? existing = Read(id);
        if (existing == null)
            throw new Exception($"Customer with ID {id} does not exist.");

        Customers.Remove(existing);
    }
}