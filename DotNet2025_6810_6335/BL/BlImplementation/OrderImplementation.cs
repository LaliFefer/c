namespace BlImplementation;
using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Linq;

internal class OrderImplementation : IOrder
{
    private DalApi.IDal _dal = DalApi.Factory.Get;

    public IEnumerable<BO.SaleInProduct> AddProductToOrder(BO.Order order, int productId, int quantity)
    {
        if (order is null)
            throw new BO.BlInvalidInputException("Order cannot be null");

        if (quantity <= 0)
            throw new BO.BlInvalidInputException("Quantity must be greater than zero");

        var doProduct = _dal.Product.Read(productId) ?? throw new BO.BlNotFoundException($"Product ID {productId} not found");
        if (doProduct.QuantityInStock < quantity)
            throw new BO.BlNotEnoughInStockException($"Not enough stock for product {productId}");

        var products = order.Products?.ToList() ?? new List<BO.ProductInOrder>();
        var productInOrder = products.FirstOrDefault(p => p.ProductID == productId);

        if (productInOrder != null)
        {
            productInOrder.Quantity += quantity;
        }
        else
        {
            productInOrder = new BO.ProductInOrder
            {
                ProductID = productId,
                ProductName = doProduct.ProductName,
                BasePrice = doProduct.Price,
                Quantity = quantity
            };
            products.Add(productInOrder);
        }

        SearchSaleForProduct(productInOrder, order.IsClubCustomer);
        CalcTotalPriceForProduct(productInOrder);
        order.Products = products;
        CalcTotalPrice(order);

        return productInOrder.Sales ?? Enumerable.Empty<BO.SaleInProduct>();
    }

    public void CalcTotalPriceForProduct(BO.ProductInOrder productInOrder)
    {
        if (productInOrder is null)
            throw new BO.BlInvalidInputException("Product in order cannot be null");

        int count = productInOrder.Quantity;
        double total = 0;
        var usedSales = new List<BO.SaleInProduct>();

        foreach (var sale in productInOrder.Sales?.OrderBy(s => s.Price / s.QuantityForSale) ?? Enumerable.Empty<BO.SaleInProduct>())
        {
            if (count < sale.QuantityForSale)
                continue;

            int times = count / sale.QuantityForSale;
            if (times <= 0)
                continue;

            total += times * sale.Price;
            count -= times * sale.QuantityForSale;
            usedSales.Add(sale);

            if (count == 0)
                break;
        }

        total += count * productInOrder.BasePrice;
        productInOrder.Sales = usedSales;
        productInOrder.TotalPrice = total;
    }

    public void CalcTotalPrice(BO.Order order)
    {
        if (order is null)
            throw new BO.BlInvalidInputException("Order cannot be null");

        order.TotalPrice = order.Products?.Sum(p => p.TotalPrice) ?? 0;
    }

    public void DoOrder(BO.Order order)
    {
        if (order is null)
            throw new BO.BlInvalidInputException("Order cannot be null");

        var products = order.Products?.ToList() ?? new List<BO.ProductInOrder>();

        foreach (var product in products)
        {
            var doProduct = _dal.Product.Read(product.ProductID) ?? throw new BO.BlNotFoundException($"Product ID {product.ProductID} not found");
            if (product.Quantity > doProduct.QuantityInStock)
                throw new BO.BlNotEnoughInStockException($"Not enough stock for product {product.ProductID}");

            _dal.Product.Update(new DO.Product(
                doProduct.IDNumber,
                doProduct.ProductName,
                doProduct.Category,
                doProduct.Price,
                doProduct.QuantityInStock - product.Quantity
            ));
        }
    }

    public void SearchSaleForProduct(BO.ProductInOrder productInOrder, bool isClubCustomer)
    {
        if (productInOrder is null)
            throw new BO.BlInvalidInputException("Product in order cannot be null");

        productInOrder.Sales = _dal.Sale.ReadAll(sale => sale != null && sale.ProductIDNumber == productInOrder.ProductID)
            .Where(sale => sale != null)
            .Cast<DO.Sale>()
            .Where(sale =>
            {
                bool validStart = DateTime.TryParse(sale.SaleStartDate, out var start);
                bool validEnd = DateTime.TryParse(sale.SaleEndDate, out var end);
                bool inPeriod = validStart && validEnd && DateTime.Now >= start && DateTime.Now <= end;
                return inPeriod
                    && productInOrder.Quantity >= sale.QuantityItemsRequiredtoReceivetheSale
                    && (!sale.SaleOnlyforClubCustomers || isClubCustomer);
            })
            .Select(sale => new BO.SaleInProduct
            {
                IDNumber = sale.IDNumber,
                QuantityForSale = sale.QuantityItemsRequiredtoReceivetheSale,
                Price = sale.FullPrice,
                ForAllCustomers = !sale.SaleOnlyforClubCustomers
            })
            .OrderBy(sale => sale.Price / sale.QuantityForSale)
            .ToList();
    }
}
