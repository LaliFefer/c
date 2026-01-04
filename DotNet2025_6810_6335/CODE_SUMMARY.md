תיעוד קצר וממוקד של השורות/מבנים העיקריים בפרויקט (מטרת כל פריט בשורה אחת)

DalList/DataSource.cs
- internal static List<Customer?> Customers = new();    // רשימת לקוחות בזיכרון
- internal static List<Product?> Products = new();      // רשימת מוצרים בזיכרון
- internal static List<Sale?> Sales = new();            // רשימת מכירות בזיכרון
- internal static class Config { internal static int NextProductId => ... }  // מחולל מזהי רץ

DalFacede/DO
- public enum Categories { MEN, WOMEN, KIDS, SPORTS, ELEGANT }   // קטגוריות מוצרים
- public record Product(int IDNumber, string ProductName, Categories Category, double Price, int QuantityInStock)   // ייצוג מוצר
- public record Customer(int IDNumber, string CustomerName, string EmailAddress, string TelephoneNumber)         // ייצוג לקוח
- public record Sale(int IDNumber, int ProductIDNumber, int QuantityItemsRequiredtoReceivetheSale, double FullPrice, bool SaleOnlyforClubCustomers, string SaleStartDate, string SaleEndDate) // ייצוג מבצע

DalFacede/DalApi
- public interface ICrud<T> { Create/Read/ReadAll/Update/Delete }   // ממשק גנרי ל-CRUD
- public interface IProduct : ICrud<Product>  // CRUD למוצרים
- public interface ICustomer : ICrud<Customer> // CRUD ללקוחות
- public interface ISale : ICrud<Sale>        // CRUD למכירות
- public interface IDal { IProduct Product; ICustomer Customer; ISale Sale; } // ממשק מאחד ל-DAL

DalList (מימושי DAL)
- DalList : IDal  // מפעל שמחזיר מימושים
- ProductImplementation.Create/Read/ReadAll/Update/Delete  // מימוש CRUD למוצרים (Create מקצה id רץ)
- CustomerImplementation.Create/Read/ReadAll/Update/Delete // מימוש CRUD ללקוחות (Create בודק אם id קיים)
- SaleImplementation.Create/Read/ReadAll/Update/Delete     // מימוש CRUD למכירות (Create מקצה id רץ)

DalTest
- Initialization.Initialize(IDal dal) // אתחול נתוני דמה בעזרת מופע IDal אחד
- Program.Main()  // נקודת כניסה: אתחול והפעלת תפריט בדיקה קונסולי
- MainLoop() / EntityMenu<T>(ICrud<T> repo, string name) // תפריט ראשי ותתי תפריטים גנריים ל-CRUD
- ReadIntFromConsole / ReadDoubleFromConsole / ReadStringFromConsole / ReadDateFromConsole // עזרי קלט (TryParse)
- ReadProductFromConsole / ReadProductFromConsoleForUpdate // קריאה ליצירה ולעדכון מוצר
- ReadCustomerFromConsole / ReadCustomerFromConsoleForUpdate // קריאה ליצירה ולעדכון לקוח
- ReadSaleFromConsole / ReadSaleFromConsoleForUpdate       // קריאה ליצירה ולעדכון מכירה

התנהגות חריגות
- DAL יזרוק Exception כאשר מנסים לעדכן/למחוק לפי id שלא קיים; ה-UI תופס ומדפיס את ההודעה.

הערה כללית
- הנתונים נשמרים בזיכרון בלבד (אין persistence). המזהים הרצים מנוהלים ב-DataSource.Config.
