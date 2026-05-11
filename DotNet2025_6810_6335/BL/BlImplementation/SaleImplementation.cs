namespace BlImplementation;
using BlApi;
using BO;
using DalFacade.DalExceptions;
using System;
using System.Collections.Generic;
using System.Linq;

internal class SaleImplementation : ISale
{
    private DalApi.IDal _dal = DalApi.Factory.Get;

    public IEnumerable<BO.Sale> GetList()
    {
        try
        {
            return _dal.Sale.ReadAll()
                .Where(doSale => doSale != null)
                .Cast<DO.Sale>()
                .Select(doSale => doSale.ToBO()!);
        }
        catch (Exception ex)
        {
            throw new BO.BlInvalidInputException("Failed to retrieve sale list", ex);
        }
    }

    public BO.Sale GetById(int id)
    {
        if (id <= 0)
            throw new BO.BlInvalidInputException("Sale ID must be positive");

        try
        {
            var doSale = _dal.Sale.Read(id) ?? throw new BO.BlNotFoundException($"Sale with ID {id} not found");
            return doSale.ToBO()!;
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BO.BlNotFoundException($"Sale with ID {id} not found", ex);
        }
        catch (Exception ex) when (ex is not BO.BlNotFoundException)
        {
            throw new BO.BlInvalidInputException($"Failed to retrieve sale {id}", ex);
        }
    }

    public void Add(BO.Sale sale)
    {
        if (sale is null)
            throw new BO.BlInvalidInputException("Sale cannot be null");
        if (sale.IDNumber <= 0)
            throw new BO.BlInvalidInputException("Invalid sale ID");

        try
        {
            if (_dal.Sale.Read(sale.IDNumber) != null)
                throw new BO.BlAlreadyExistsException($"Sale ID {sale.IDNumber} already exists");

            _dal.Sale.Create(sale.ToDO());
        }
        catch (DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Sale ID {sale.IDNumber} already exists", ex);
        }
        catch (Exception ex) when (ex is not BO.BlAlreadyExistsException)
        {
            throw new BO.BlInvalidInputException("Failed to add sale", ex);
        }
    }

    public void Update(BO.Sale sale)
    {
        if (sale is null)
            throw new BO.BlInvalidInputException("Sale cannot be null");

        try
        {
            var existing = _dal.Sale.Read(sale.IDNumber) ?? throw new BO.BlNotFoundException($"Sale ID {sale.IDNumber} not found");
            _dal.Sale.Update(sale.ToDO());
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BO.BlNotFoundException($"Sale ID {sale.IDNumber} not found", ex);
        }
        catch (Exception ex) when (ex is not BO.BlNotFoundException)
        {
            throw new BO.BlInvalidInputException("Failed to update sale", ex);
        }
    }

    public void Delete(int id)
    {
        if (id <= 0)
            throw new BO.BlInvalidInputException("Sale ID must be positive");

        try
        {
            if (_dal.Sale.Read(id) == null)
                throw new BO.BlNotFoundException($"Sale ID {id} not found");
            _dal.Sale.Delete(id);
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BO.BlNotFoundException($"Sale ID {id} not found", ex);
        }
        catch (Exception ex) when (ex is not BO.BlNotFoundException)
        {
            throw new BO.BlInvalidInputException("Failed to delete sale", ex);
        }
    }
}