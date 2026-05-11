// דוגמת תוכנית ראשית לבדיקת ה-DAL
using Dal;
using DalTest;
using DalApi;
using DO;
using System;
using System.Reflection;
using Tools;
using DalFacade.DalExceptions;

namespace DalTest
{
    public class Program
    {
        // שדה סטטי לקריאה בלבד שמכיל את ה-DAL המאוחד (מבוקש בשלב4)
        private static readonly IDal s_dal = DalApi.Factory.Get;
        public static void Main()
        {
        // עטיפה חיצונית לתפיסת חריגות מה-DAL
            try
            {
                // אתחול בסיס הנתונים בעזרת ה-IDal המאוחד
                // Initialization.Initialize now uses Factory to get the IDal

                Console.Write("Initialize data from scratch? (y/N): ");
                string? initChoice = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(initChoice) && (initChoice.Equals("y", StringComparison.OrdinalIgnoreCase) || initChoice.Equals("yes", StringComparison.OrdinalIgnoreCase)))
                {
                    Initialization.Initialize();
                    Console.WriteLine("Initialization completed.");
                }
                else
                {
                    Console.WriteLine("Skipping initialization. Existing XML data will be used.");
                }

                // לולאת תפריט ראשי
                MainLoop();
            }
            catch (DalAlreadyExistsException ex)
            {
                Console.WriteLine("DAL error - already exists: " + ex.Message);
                var mb = MethodBase.GetCurrentMethod();
                LogManager.Log(mb?.DeclaringType?.FullName ?? "", mb?.Name ?? "", ex.ToString());
            }
            catch (DalDoesNotExistException ex)
            {
                Console.WriteLine("DAL error - not found: " + ex.Message);
                var mb = MethodBase.GetCurrentMethod();
                LogManager.Log(mb?.DeclaringType?.FullName ?? "", mb?.Name ?? "", ex.ToString());
            }
            catch (Exception ex)
            {
                // הדפסת החריגה שנתפסה (לפי הוראות שלב3)
                Console.WriteLine("Unhandled exception: " + ex);
                var mb = MethodBase.GetCurrentMethod();
                LogManager.Log(mb?.DeclaringType?.FullName ?? "", mb?.Name ?? "", ex.ToString());
            }
        }

        // לולאת תפריט ראשי שמציגה אפשרויות לבדיקת כל ישות
        private static void MainLoop()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Main Menu:");
                Console.WriteLine("1 - Products");
                Console.WriteLine("2 - Customers");
                Console.WriteLine("3 - Sales");
                Console.WriteLine("4 - ניקוי קבצי לוג ישנים");
                Console.WriteLine("0 - Exit");
                Console.Write("Choose entity: ");

                string? choice = Console.ReadLine();
                if (choice == "0")
                {
                    Console.WriteLine("Bye:)\r\nYou've left our software... See you next time!");
                    break;
                }

                switch (choice)
                {
                    case "1": EntityMenu(s_dal.Product, "Product"); break;
                    case "2": EntityMenu(s_dal.Customer, "Customer"); break;
                    case "3": EntityMenu(s_dal.Sale, "Sale"); break;
                    case "4": CleanOldLogFiles(); break;
                    default: Console.WriteLine("Invalid choice"); break;
                }
            }
        }

        private static void CleanOldLogFiles()
        {
            try
            {
                LogManager.CleanOldLogs();
                Console.WriteLine("ניקוי קבצי הלוג הישנים הושלם.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("שגיאה בניקוי לוגים: " + ex.Message);
                var mb = MethodBase.GetCurrentMethod();
                LogManager.Log(mb?.DeclaringType?.FullName ?? "", mb?.Name ?? "", ex.ToString());
            }
        }

        // תת-תפריט גנרי עבור CRUD על ישות
        private static void EntityMenu<T>(ICrud<T> repo, string name) where T : class
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine($"{name} Menu:");
                Console.WriteLine("1 - Create");
                Console.WriteLine("2 - Read by ID");
                Console.WriteLine("3 - Read All");
                Console.WriteLine("4 - Update");
                Console.WriteLine("5 - Delete");
                Console.WriteLine("0 - Back");
                Console.Write("Choose action: ");

                string? action = Console.ReadLine();
                if (action == "0")
                {
                    Console.WriteLine("Bye:)\r\nYou've left our software... See you next time!");
                    break;
                }
                

                try
                {
                    switch (action)
                    {
                        case "1": // Create
                            if (typeof(T) == typeof(Product))
                            {
                                var p = ReadProductFromConsole();
                                int id = ((ICrud<Product>)repo).Create(p);
                                Console.WriteLine($"Created Product id: {id}");
                            }
                            else if (typeof(T) == typeof(Customer))
                            {
                                var c = ReadCustomerFromConsole();
                                int id = ((ICrud<Customer>)repo).Create(c);
                                Console.WriteLine($"Created Customer id: {id}");
                            }
                            else if (typeof(T) == typeof(Sale))
                            {
                                var s = ReadSaleFromConsole();
                                int id = ((ICrud<Sale>)repo).Create(s);
                                Console.WriteLine($"Created Sale id: {id}");
                            }
                            else Console.WriteLine("Create not supported for this type");
                            break;

                        case "2": // Read by ID
                            int rid = ReadIntFromConsole("Enter id: ");
                            var item = repo.Read(rid);
                            Console.WriteLine(item == null ? "Not found" : item.ToString());
                            break;

                        case "3": // Read All
                            var list = repo.ReadAll();
                            Console.WriteLine($"{name} list count: {list.Count}");
                            foreach (var it in list) Console.WriteLine(it);
                            break;

                        case "4": // Update
                            if (typeof(T) == typeof(Product))
                            {
                                var p = ReadProductFromConsole();
                                ((ICrud<Product>)repo).Update(p);
                                Console.WriteLine("Product updated");
                            }
                            else if (typeof(T) == typeof(Customer))
                            {
                                var c = ReadCustomerFromConsole();
                                ((ICrud<Customer>)repo).Update(c);
                                Console.WriteLine("Customer updated");
                            }
                            else if (typeof(T) == typeof(Sale))
                            {
                                var s = ReadSaleFromConsole();
                                ((ICrud<Sale>)repo).Update(s);
                                Console.WriteLine("Sale updated");
                            }
                            else Console.WriteLine("Update not supported for this type");
                            break;

                        case "5": // Delete
                            int did = ReadIntFromConsole("Enter id to delete: ");
                            repo.Delete(did);
                            Console.WriteLine($"Deleted {name} with id {did}");
                            break;

                        default:
                            Console.WriteLine("Invalid action");
                            break;
                    }
                }
                catch (DalAlreadyExistsException ex)
                {
                    Console.WriteLine("DAL Error (already exists): " + ex.Message);
                    var mb = MethodBase.GetCurrentMethod();
                    LogManager.Log(mb?.DeclaringType?.FullName ?? "", mb?.Name ?? "", ex.ToString());
                }
                catch (DalDoesNotExistException ex)
                {
                    Console.WriteLine("DAL Error (not found): " + ex.Message);
                    var mb = MethodBase.GetCurrentMethod();
                    LogManager.Log(mb?.DeclaringType?.FullName ?? "", mb?.Name ?? "", ex.ToString());
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine("Argument error: " + ex.Message);
                    var mb = MethodBase.GetCurrentMethod();
                    LogManager.Log(mb?.DeclaringType?.FullName ?? "", mb?.Name ?? "", ex.ToString());
                }
                catch (Exception ex)
                {
                    // הדפסת החריגה שנתפסה (לפי הוראות)
                    Console.WriteLine("Exception: " + ex.Message);
                    var mb = MethodBase.GetCurrentMethod();
                    LogManager.Log(mb?.DeclaringType?.FullName ?? "", mb?.Name ?? "", ex.ToString());
                }
            }
        }
        private static int ReadIntFromConsole(string prompt)
        {
            Console.Write(prompt);
            string? s = Console.ReadLine();
            int.TryParse(s, out int v);
            return v; // אין בדיקות נוספות לפי ההנחיות
        }

        private static double ReadDoubleFromConsole(string prompt)
        {
            Console.Write(prompt);
            string? s = Console.ReadLine();
            double.TryParse(s, out double v);
            return v;
        }

        private static string ReadStringFromConsole(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? string.Empty;
        }

        private static DateTime ReadDateFromConsole(string prompt)
        {
            Console.Write(prompt);
            string? s = Console.ReadLine();
            DateTime.TryParse(s, out DateTime d);
            return d;
        }

        // קריאת מוצר מהמשתמש (אין בדיקות תקינות מעבר ל-TryParse)
        private static Product ReadProductFromConsole()
        {
            // אצל Products המזהה מוקצה אוטומטית ב-DAL, שולחים 0
            string name = ReadStringFromConsole("Product name: ");
            Console.WriteLine("Categories: 0=MEN 1=WOMEN 2=KIDS 3=SPORTS 4=ELEGANT");
            int cat = ReadIntFromConsole("Category (number): ");
            double price = ReadDoubleFromConsole("Price: ");
            int qty = ReadIntFromConsole("QuantityInStock: ");
            Categories category = Categories.MEN;
            if (Enum.IsDefined(typeof(Categories), cat)) category = (Categories)cat;
            return new Product(0, name, category, price, qty);
        }

        // קריאת לקוח מהמשתמש
        private static Customer ReadCustomerFromConsole()
        {
            int id = ReadIntFromConsole("Customer ID (int): ");
            string name = ReadStringFromConsole("Customer name: ");
            string email = ReadStringFromConsole("Email: ");
            string phone = ReadStringFromConsole("Phone: ");
            return new Customer(id, name, email, phone);
        }

        // קריאת מבצע/מכירה מהמשתמש
        private static Sale ReadSaleFromConsole()
        {
            // Sale מזהה מוקצה ב-DAL, שולחים 0
            int productId = ReadIntFromConsole("Product ID: ");
            int qty = ReadIntFromConsole("QuantityItemsRequiredtoReceivetheSale: ");
            double fullPrice = ReadDoubleFromConsole("FullPrice: ");
            string forClub = ReadStringFromConsole("Sale only for club customers? (true/false): ");
            bool onlyClub = bool.TryParse(forClub, out bool b) && b;
            string start = ReadStringFromConsole("Sale start date (ISO or empty): ");
            string end = ReadStringFromConsole("Sale end date (ISO or empty): ");
            return new Sale(0, productId, qty, fullPrice, onlyClub, start, end);
        }

        // פונקציות גנריות לעבודה עם ICrud<T>
        private static void PrintAll<T>(ICrud<T> repo, string name) where T : class
        {
            var list = repo.ReadAll();
            Console.WriteLine($"{name}: {list.Count}");
        }

        private static void PrintById<T>(ICrud<T> repo, int id, string name) where T : class
        {
            var item = repo.Read(id);
            Console.WriteLine($"{name} with id {id}: {item}");
        }

        private static void DeleteById<T>(ICrud<T> repo, int id, string name) where T : class
        {
            repo.Delete(id);
            Console.WriteLine($"Deleted {name} with id {id}");
        }
    }
}
