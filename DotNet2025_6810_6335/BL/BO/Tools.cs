using System.Reflection;
using System.Linq;

namespace BO;

internal static class Tools
{
    // מתודת הרחבה (Extension Method) שעוברת על כל המאפיינים של האובייקט
    public static string ToStringProperty<T>(this T obj)
    {
        if (obj == null) return "";

        string str = "";
        // מעבר על כל ה-Properties של המחלקה בעזרת Reflection
        foreach (PropertyInfo prop in obj.GetType().GetProperties())
        {
            var value = prop.GetValue(obj, null);

            // בדיקה אם המאפיין הוא רשימה (כמו רשימת המבצעים במוצר)
            if (value is System.Collections.IEnumerable list && !(value is string))
            {
                str += "\n" + prop.Name + ": " + string.Join(", ", list.Cast<object>());
            }
            else
            {
                str += "\n" + prop.Name + ": " + (value ?? "null");
            }
        }
        return str;
    }

    public static BO.Customer DoToBoCustomer(DO.Customer doCustomer)
    {
        return new BO.Customer
        {
            IDNumber = doCustomer.IDNumber,
            CustomerName = doCustomer.CustomerName,
            EmailAddress = doCustomer.EmailAddress,
            TelephoneNumber = doCustomer.TelephoneNumber,
            IsClubMember = doCustomer.IsClubMember
        };
    }

    public static BO.Product DoToBoProduct(DO.Product doProduct)
    {
        return new BO.Product
        {
            IDNumber = doProduct.IDNumber,
            ProductName = doProduct.ProductName,
            Category = (BO.Categories)doProduct.Category,
            Price = doProduct.Price,
            QuantityInStock = doProduct.QuantityInStock
        };
    }

    public static BO.Sale DoToBoSale(DO.Sale doSale)
    {
        return new BO.Sale
        {
            IDNumber = doSale.IDNumber,
            ProductIDNumber = doSale.ProductIDNumber,
            QuantityItemsRequiredtoReceivetheSale = doSale.QuantityItemsRequiredtoReceivetheSale,
            FullPrice = doSale.FullPrice,
            SaleOnlyforClubCustomers = doSale.SaleOnlyforClubCustomers,
            SaleStartDate = doSale.SaleStartDate,
            SaleEndDate = doSale.SaleEndDate
        };
    }

    public static DO.Customer BoToDoCustomer(BO.Customer boCustomer)
    {
        return new DO.Customer(
            boCustomer.IDNumber,
            boCustomer.CustomerName,
            boCustomer.EmailAddress,
            boCustomer.TelephoneNumber,
            boCustomer.IsClubMember
        );
    }

    public static DO.Product BoToDoProduct(BO.Product boProduct)
    {
        return new DO.Product(
            boProduct.IDNumber,
            boProduct.ProductName,
            (DO.Categories)boProduct.Category,
            boProduct.Price,
            boProduct.QuantityInStock
        );
    }

    public static DO.Sale BoToDoSale(BO.Sale boSale)
    {
        return new DO.Sale(
            boSale.IDNumber,
            boSale.ProductIDNumber,
            boSale.QuantityItemsRequiredtoReceivetheSale,
            boSale.FullPrice,
            boSale.SaleOnlyforClubCustomers,
            boSale.SaleStartDate,
            boSale.SaleEndDate
        );
    }

    public static BO.Customer ToBO(this DO.Customer doCustomer) => DoToBoCustomer(doCustomer);
    public static DO.Customer ToDO(this BO.Customer boCustomer) => BoToDoCustomer(boCustomer);
    public static BO.Product ToBO(this DO.Product doProduct) => DoToBoProduct(doProduct);
    public static DO.Product ToDO(this BO.Product boProduct) => BoToDoProduct(boProduct);
    public static BO.Sale ToBO(this DO.Sale doSale) => DoToBoSale(doSale);
    public static DO.Sale ToDO(this BO.Sale boSale) => BoToDoSale(boSale);
}
