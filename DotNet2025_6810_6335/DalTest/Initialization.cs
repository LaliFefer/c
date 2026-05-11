using System;
using DalApi;
using DO;

namespace DalTest
{
    public static class Initialization
    {
        // ��� ����� �� �-DAL ������
        private static IDal? s_dal;

        // ����� ������� ������ ������� - ��� ��� �����, ����� ����� �-Factory
        public static void Initialize()
        {
            s_dal = DalApi.Factory.Get;

            createProducts();
            createCustomers();
            createSales();
        }

        // ����� ������ ������
        private static void createProducts()
        {
            var products = new[]
            {
                new Product(0, "T-Shirt", Categories.MEN, 49.9, 10),
                new Product(0, "Dress", Categories.WOMEN, 129.9, 5),
                new Product(0, "Sneakers", Categories.SPORTS, 199.0, 8),
                new Product(0, "Socks", Categories.KIDS, 9.99, 20),
                new Product(0, "Elegant Shoe", Categories.ELEGANT, 249.0, 2)
            };

            foreach (var product in products)
            {
                try
                {
                    s_dal!.Product.Create(product);
                }
                catch (DalFacade.DalExceptions.DalAlreadyExistsException)
                {
                    // Ignore already-initialized entries.
                }
            }
        }

        // ����� ������ ������
        private static void createCustomers()
        {
            var customers = new[]
            {
                new Customer(10000001, "Alice", "alice@example.com", "050-1111111"),
                new Customer(10000002, "Bob", "bob@example.com", "050-2222222"),
                new Customer(10000003, "Carol", "carol@example.com", "050-3333333")
            };

            foreach (var customer in customers)
            {
                try
                {
                    s_dal!.Customer.Create(customer);
                }
                catch (DalFacade.DalExceptions.DalAlreadyExistsException)
                {
                    // Ignore already-initialized entries.
                }
            }
        }

        // ����� ������ ������
        private static void createSales()
        {
            var products = s_dal!.Product.ReadAll();
            if (products.Count == 0) return;

            var product = products[0] ?? throw new InvalidOperationException("Product is null");
            int prodId = product.IDNumber;
            try
            {
                s_dal!.Sale.Create(new Sale(0, prodId, 1, product.Price, false, DateTime.Now.ToString("s"), DateTime.Now.AddDays(7).ToString("s")));
            }
            catch (DalFacade.DalExceptions.DalAlreadyExistsException)
            {
                // Ignore already-initialized sale entries.
            }
        }
    }
}
