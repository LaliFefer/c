namespace BlImplementation;
using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Linq;

internal class ProductImplementation : IProduct
{
    private DalApi.IDal _dal = DalApi.Factory.Get;

    public IEnumerable<BO.Product> GetList()
    {
        try
        {
            return _dal.Product.ReadAll()
                .Where(doProd => doProd != null)
                .Cast<DO.Product>()
                .Select(doProd => doProd.ToBO()!);
        }
        catch (Exception ex)
        {
            throw new BO.BlInvalidInputException("Failed to retrieve product list", ex);
        }
    }

    public BO.Product GetById(int id)
    {
        try
        {
            var doProd = _dal.Product.Read(id) ?? throw new BO.BlNotFoundException($"Product with ID {id} not found");
            return doProd.ToBO()!;
        }
        catch (BO.BlNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BO.BlInvalidInputException("Failed to retrieve product", ex);
        }
    }

    public void Add(BO.Product product)
    {
        if (product is null)
            throw new BO.BlInvalidInputException("Product cannot be null");

        try
        {
            // if ID provided (>0) check existence, otherwise let DAL assign
            if (product.IDNumber > 0)
            {
                if (_dal.Product.Read(product.IDNumber) != null)
                    throw new BO.BlAlreadyExistsException($"Product ID {product.IDNumber} already exists");

                _dal.Product.Create(new DO.Product(
                    product.IDNumber,
                    product.ProductName,
                    (DO.Categories)product.Category,
                    product.Price,
                    product.QuantityInStock
                ));
            }
            else
            {
                // let DAL assign ID by passing 0
                _dal.Product.Create(new DO.Product(
                    0,
                    product.ProductName,
                    (DO.Categories)product.Category,
                    product.Price,
                    product.QuantityInStock
                ));
            }
        }
        catch (BO.BlAlreadyExistsException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BO.BlInvalidInputException("Failed to add product", ex);
        }
    }

    public void Update(BO.Product product)
    {
        if (product is null)
            throw new BO.BlInvalidInputException("Product cannot be null");

        try
        {
            var existing = _dal.Product.Read(product.IDNumber) ?? throw new BO.BlNotFoundException($"Product ID {product.IDNumber} not found");
            _dal.Product.Update(new DO.Product(
                existing.IDNumber,
                product.ProductName,
                (DO.Categories)product.Category,
                product.Price,
                product.QuantityInStock
            ));
        }
        catch (BO.BlNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BO.BlInvalidInputException("Failed to update product", ex);
        }
    }

    public void Delete(int id)
    {
        try
        {
            if (_dal.Product.Read(id) == null)
                throw new BO.BlNotFoundException($"Product ID {id} not found");
            _dal.Product.Delete(id);
        }
        catch (BO.BlNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BO.BlInvalidInputException("Failed to delete product", ex);
        }
    }
}