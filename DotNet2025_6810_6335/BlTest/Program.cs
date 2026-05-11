using System;
using System.Collections.Generic;
using System.Linq;
using DalTest;

namespace BlTest
{
    internal class Program
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        static void Main(string[] args)
        {
            DalTest.Initialization.Initialize();
            Console.WriteLine("נתוני DAL אתחלו בהצלחה. לחץ ENTER להמשך...");
            Console.ReadLine();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("BL Test - שלב 8\n");
                Console.WriteLine("1. הצג את כל המוצרים");
                Console.WriteLine("2. הצג את כל המבצעים");
                Console.WriteLine("3. הצג לקוח לפי ID");
                Console.WriteLine("4. הוסף מוצר חדש");
                Console.WriteLine("5. עדכן מוצר קיים");
                Console.WriteLine("6. בצע הזמנה לדוגמה");
                Console.WriteLine("7. הרץ תהליך הזמנה מלא עם בדיקת מלאי");
                Console.WriteLine("0. יציאה");
                Console.Write("בחירה: ");
                string? input = Console.ReadLine();
                Console.WriteLine();

                try
                {
                    switch (input)
                    {
                        case "1":
                            ShowProducts();
                            break;
                        case "2":
                            ShowSales();
                            break;
                        case "3":
                            ShowCustomer();
                            break;
                        case "4":
                            AddProduct();
                            break;
                        case "5":
                            UpdateProduct();
                            break;
                        case "6":
                            TestOrder();
                            break;
                        case "7":
                            RunOrderSimulation();
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("בחירה לא תקינה");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"שגיאה: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"- {ex.InnerException.Message}");
                }

                Console.WriteLine("\nלחץ ENTER להמשך...");
                Console.ReadLine();
            }
        }

        private static void ShowProducts()
        {
            foreach (var product in s_bl.Product.GetList())
            {
                Console.WriteLine(product);
                Console.WriteLine(new string('-', 40));
            }
        }

        private static void ShowSales()
        {
            foreach (var sale in s_bl.Sale.GetList())
            {
                Console.WriteLine(sale);
                Console.WriteLine(new string('-', 40));
            }
        }

        private static void ShowCustomer()
        {
            Console.Write("הכנס מספר לקוח: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var customer = s_bl.Customer.GetById(id);
                Console.WriteLine(customer);
            }
            else
            {
                Console.WriteLine("מספר לא חוקי");
            }
        }

        private static void AddProduct()
        {
            Console.Write("הכנס מזהה מוצר: ");
            int id = int.Parse(Console.ReadLine() ?? "0");
            Console.Write("שם מוצר: ");
            string name = Console.ReadLine() ?? string.Empty;
            Console.Write("קטגוריה (MEN,WOMEN,KIDS,SPORTS,ELEGANT): ");
            var category = Enum.Parse<BO.Categories>(Console.ReadLine() ?? "MEN");
            Console.Write("מחיר: ");
            double price = double.Parse(Console.ReadLine() ?? "0");
            Console.Write("כמות במלאי: ");
            int quantity = int.Parse(Console.ReadLine() ?? "0");

            s_bl.Product.Add(new BO.Product
            {
                IDNumber = id,
                ProductName = name,
                Category = category,
                Price = price,
                QuantityInStock = quantity
            });

            Console.WriteLine("מוצר נוסף בהצלחה");
        }

        private static void UpdateProduct()
        {
            Console.Write("הכנס מזהה מוצר לעדכון: ");
            int id = int.Parse(Console.ReadLine() ?? "0");
            var existing = s_bl.Product.GetById(id);
            Console.WriteLine($"מוצר קיים: {existing.ProductName}, מלאי: {existing.QuantityInStock}");
            Console.Write("כמות חדשה במלאי: ");
            existing.QuantityInStock = int.Parse(Console.ReadLine() ?? "0");
            s_bl.Product.Update(existing);
            Console.WriteLine("עדכון מוצר בוצע בהצלחה");
        }

        private static void TestOrder()
        {
            var order = new BO.Order { IsClubCustomer = false, Products = new List<BO.ProductInOrder>() };
            Console.Write("הכנס מזהה מוצר להזמנה: ");
            int productId = int.Parse(Console.ReadLine() ?? "0");
            Console.Write("כמות להזמנה: ");
            int quantity = int.Parse(Console.ReadLine() ?? "0");

            var sales = s_bl.Order.AddProductToOrder(order, productId, quantity);
            Console.WriteLine("מבצעים שנמצאו:");
            foreach (var sale in sales)
            {
                Console.WriteLine(sale);
            }

            Console.WriteLine($"סכום ביניים להזמנה: {order.TotalPrice}");
            s_bl.Order.DoOrder(order);
            Console.WriteLine("ההזמנה בוצעה ועדכון מלאי נעשה");
        }

        private static void RunOrderSimulation()
        {
            var products = s_bl.Product.GetList().ToArray();
            if (!products.Any())
            {
                Console.WriteLine("אין מוצרים לביצוע הזמנה");
                return;
            }

            var selected = products.First();
            var order = new BO.Order { IsClubCustomer = false, Products = new List<BO.ProductInOrder>() };
            var sales = s_bl.Order.AddProductToOrder(order, selected.IDNumber, 1);

            Console.WriteLine("מערך הבדיקה: הוספת מוצר להזמנה");
            Console.WriteLine(selected);
            Console.WriteLine("מבצעים שנמצאו:");
            foreach (var sale in sales)
                Console.WriteLine(sale);

            Console.WriteLine($"סכום ביניים להזמנה: {order.TotalPrice}");
            s_bl.Order.DoOrder(order);
            Console.WriteLine("ההזמנה בוצעה בהצלחה");
            var after = s_bl.Product.GetById(selected.IDNumber);
            Console.WriteLine($"מלאי עדכני לאחר ההזמנה: {after.QuantityInStock}");
        }
    }
}
