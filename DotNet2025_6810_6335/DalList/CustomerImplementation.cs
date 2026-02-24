using DalApi;
using System.Reflection;
using Tools;
using DO;
using System.Collections.Generic;
using System.Linq;
using static Dal.DataSource;
using DalFacade.DalExceptions;

namespace Dal;


internal class CustomerImplementation : ICustomer
{
    // יצירת לקוח חדש
    public int Create(Customer item)
    {
        var mb = MethodBase.GetCurrentMethod();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "Start Create");

        if (item is null)
        {
            LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "ArgumentNull: item is null");
            throw new ArgumentNullException(nameof(item));
        }

        // בדיקה אם כבר קיים לקוח עם אותו מזהה (למשל תעודת זהות)
        if (Read(item.IDNumber) != null)
        {
            LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, $"AlreadyExists id={item.IDNumber}");
            throw new DalEntityAlreadyExistsException($"Customer with ID {item.IDNumber} already exists.");
        }

        Customers = Customers.Append(item).ToList();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, $"Created id={item.IDNumber}");
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "End Create");
        return item.IDNumber;
    }

    // CustomerImplementation: CRUD for Customer over DataSource

    public Customer? Read(int id) =>
        Customers.FirstOrDefault(c => c.IDNumber == id);

    // Read by predicate (stage2)
    public Customer? Read(System.Func<Customer, bool> filter) =>
        filter == null ? null : Customers.FirstOrDefault(filter);

    // ReadAll with optional filter
    public List<Customer?> ReadAll(System.Func<Customer, bool>? filter = null)
    {
        var mb = MethodBase.GetCurrentMethod();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "Start ReadAll");

        if (filter == null)
            return Customers.Select(c => (Customer?)c).ToList();

        var res = Customers.Where(filter).Select(c => (Customer?)c).ToList();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, $"End ReadAll count={res.Count}");
        return res;
    }

    public void Update(Customer item)
    {
        var mb = MethodBase.GetCurrentMethod();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "Start Update");

        if (item is null)
        {
            LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "ArgumentNull: item is null");
            throw new ArgumentNullException(nameof(item));
        }

        Customer? existing = Read(item.IDNumber);
        if (existing == null)
        {
            LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, $"NotFound id={item.IDNumber}");
            throw new DalEntityNotFoundException($"Customer with ID {item.IDNumber} does not exist.");
        }

        Customers = Customers.Select(c => c.IDNumber == item.IDNumber ? item : c).ToList();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, $"End Update id={item.IDNumber}");
    }

    public void Delete(int id)
    {
        var mb = MethodBase.GetCurrentMethod();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "Start Delete");

        Customer? existing = Read(id);
        if (existing == null)
        {
            LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, $"NotFound id={id}");
            throw new DalEntityNotFoundException($"Customer with ID {id} does not exist.");
        }

        Customers = Customers.Where(c => c.IDNumber != id).ToList();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, $"End Delete id={id}");
    }
}