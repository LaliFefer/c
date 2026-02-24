using System.Reflection;
using Tools;
namespace Dal;
using DalApi;
using DO;
using System.Linq;
using System.Collections.Generic;
using static Dal.DataSource; // מאפשר גישה ישירה לרשימות ול-Config
using DalFacade.DalExceptions;

internal class SaleImplementation : ISale
{
    // SaleImplementation: CRUD for Sale over DataSource
    // 1. יצירת מכירה חדשה
    public int Create(Sale item)
    {
        var mb = MethodBase.GetCurrentMethod();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "Start Create");

        if (item is null)
        {
            LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "ArgumentNull: item is null");
            throw new ArgumentNullException(nameof(item));
        }

        // קבלת מזהה רץ חדש מהקונפיגורציה
        int newId = Config.NextSaleId;

        // יצירת עותק חדש של הישות עם המזהה שנוצר (במידה והוא לא נקבע מראש)
        Sale newItem = item with { IDNumber = newId };

        // הוספה לרשימה בעזרת LINQ
        Sales = Sales.Append(newItem).ToList();

        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, $"Created id={newId}");
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "End Create");
        return newId;
    }

    // 2. קריאת מכירה לפי מזהה
    public Sale? Read(int id)
    {
        var mb = MethodBase.GetCurrentMethod();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, $"Read id={id}");
        // חיפוש המכירה הראשונה שמתאימה למזהה, או החזרת null אם לא נמצאה
        var res = Sales.FirstOrDefault(s => s.IDNumber == id);
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, $"End Read found={(res!=null)}");
        return res;
    }

    // Read by predicate (stage2)
    public Sale? Read(System.Func<Sale, bool> filter) =>
        filter == null ? null : Sales.FirstOrDefault(filter);

    // 3. קריאת כל המכירות
    public List<Sale?> ReadAll(System.Func<Sale, bool>? filter = null)
    {
        var mb = MethodBase.GetCurrentMethod();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "Start ReadAll");

        if (filter == null)
            return Sales.Select(s => (Sale?)s).ToList();

        var res = Sales.Where(filter).Select(s => (Sale?)s).ToList();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, $"End ReadAll count={res.Count}");
        return res;
    }

    // 4. עדכון מכירה קיימת
    public void Update(Sale item)
    {
        var mb = MethodBase.GetCurrentMethod();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "Start Update");

        if (item is null)
        {
            LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, "ArgumentNull: item is null");
            throw new ArgumentNullException(nameof(item));
        }

        // בדיקה אם המכירה קיימת - הלוגיקה היחידה המותרת
        Sale? existing = Sales.FirstOrDefault(s => s.IDNumber == item.IDNumber);

        if (existing == null)
        {
            LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, $"NotFound id={item.IDNumber}");
            throw new DalEntityNotFoundException($"Sale with ID {item.IDNumber} does not exist.");
        }

        //עדכון באמצעות Select
        Sales = Sales.Select(s => s.IDNumber == item.IDNumber ? item : s).ToList();
        LogManager.Log(mb.DeclaringType?.FullName ?? "", mb.Name, $"End Update id={item.IDNumber}");
    }

    // 5. מחיקת מכירה
    public void Delete(int id)
    {
        Sale? existing = Sales.FirstOrDefault(s => s.IDNumber == id);

        if (existing == null)
            throw new DalEntityNotFoundException($"Sale with ID {id} does not exist.");

        // מחיקה באמצעות Where
        Sales = Sales.Where(s => s.IDNumber != id).ToList();
    }
}