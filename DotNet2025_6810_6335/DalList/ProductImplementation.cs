using DalApi;
using System.Reflection;
using Tools;
using DO;
using System.Collections.Generic;
using System.Linq;
using static Dal.DataSource;
using DalFacade.DalExceptions;

namespace Dal;

internal class ProductImplementation : IProduct
{
    // ProductImplementation: CRUD for Product over DataSource
    public int Create(Product item)
    {
        var mb = MethodBase.GetCurrentMethod();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "Start Create");

        if (item is null)
        {
            LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "ArgumentNull: item is null");
            throw new ArgumentNullException(nameof(item));
        }

        int newId = Config.NextProductId;
        Product newItem = item with { IDNumber = newId };
        Products = Products.Append(newItem).ToList();

        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, $"Created id={newId}");
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "End Create");
        return newId;
    }

    public Product? Read(int id) =>
        Products.FirstOrDefault(p => p.IDNumber == id);

    // Read by predicate (stage2)
    public Product? Read(System.Func<Product, bool> filter) =>
        filter == null ? null : Products.FirstOrDefault(filter);

    // ReadAll with optional filter: returns list of nullable Product to match interface
    public List<Product?> ReadAll(System.Func<Product, bool>? filter = null)
    {
        // אם אין פילטר, החזר את כל הרשימה כמערכת של Product? (ממפה כל פריט ל-Product?)
        var mb = MethodBase.GetCurrentMethod();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "Start ReadAll");

        if (filter == null)
            return Products.Select(p => (Product?)p).ToList();

        // אם יש פילטר, החזר רק את הפריטים שעבורם הפילטר מחזיר true
        var res = Products.Where(filter).Select(p => (Product?)p).ToList();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, $"End ReadAll count={res.Count}");
        return res;
    }

    public void Update(Product item)
    {
        var mb = MethodBase.GetCurrentMethod();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "Start Update");

        if (item is null)
        {
            LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "ArgumentNull: item is null");
            throw new ArgumentNullException(nameof(item));
        }

        // ensure entity exists
        Product? existing = Read(item.IDNumber);
        if (existing == null)
        {
            LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, $"NotFound id={item.IDNumber}");
            throw new DalEntityNotFoundException($"Product with ID {item.IDNumber} does not exist.");
        }

        // replace the matching item using LINQ Select and reassign the backing list
        Products = Products.Select(p => p.IDNumber == item.IDNumber ? item : p).ToList();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, $"End Update id={item.IDNumber}");
    }

    public void Delete(int id)
    {
        var mb = MethodBase.GetCurrentMethod();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "Start Delete");

        Product? existing = Read(id);
        if (existing == null)
        {
            LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, $"NotFound id={id}");
            throw new DalEntityNotFoundException($"Product with ID {id} does not exist.");
        }

        // remove by filtering with LINQ Where and reassign backing list
        Products = Products.Where(p => p.IDNumber != id).ToList();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, $"End Delete id={id}");
    }
}