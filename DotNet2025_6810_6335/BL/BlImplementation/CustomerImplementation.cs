namespace BlImplementation;
using BlApi;
using BO;
using DalFacade.DalExceptions;
using System;
using System.Linq;
using System.Collections.Generic;

internal class CustomerImplementation : ICustomer
{
    private DalApi.IDal _dal = DalApi.Factory.Get;

    public IEnumerable<BO.Customer> GetList()
    {
        try
        {
            return _dal.Customer.ReadAll()
                .Where(c => c != null)
                .Cast<DO.Customer>()
                .Select(d => d.ToBO());
        }
        catch (Exception ex)
        {
            throw new BO.BlInvalidInputException("Failed to retrieve customer list", ex);
        }
    }

    public BO.Customer GetById(int id)
    {
        if (id <= 0)
            throw new BO.BlInvalidInputException("Customer ID must be positive");

        try
        {
            var doCust = _dal.Customer.Read(id) ?? throw new BO.BlNotFoundException($"Customer with ID {id} not found");
            return doCust.ToBO()!;
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BO.BlNotFoundException($"Customer with ID {id} not found", ex);
        }
        catch (Exception ex) when (ex is not BO.BlNotFoundException)
        {
            throw new BO.BlInvalidInputException($"Failed to retrieve customer ID {id}", ex);
        }
    }

    public void Add(BO.Customer customer)
    {
        if (customer is null)
            throw new BO.BlInvalidInputException("Customer cannot be null");
        if (customer.IDNumber <= 0)
            throw new BO.BlInvalidInputException("Invalid customer ID number");

        try
        {
            if (_dal.Customer.Read(customer.IDNumber) != null)
                throw new BO.BlAlreadyExistsException($"Customer ID {customer.IDNumber} already exists");

            _dal.Customer.Create(customer.ToDO());
        }
        catch (DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Customer ID {customer.IDNumber} already exists", ex);
        }
        catch (Exception ex) when (ex is not BO.BlAlreadyExistsException)
        {
            throw new BO.BlInvalidInputException("Failed to add customer", ex);
        }
    }

    public void Update(BO.Customer customer)
    {
        if (customer is null)
            throw new BO.BlInvalidInputException("Customer cannot be null");

        try
        {
            if (_dal.Customer.Read(customer.IDNumber) == null)
                throw new BO.BlNotFoundException($"Customer ID {customer.IDNumber} not found");

            _dal.Customer.Update(customer.ToDO());
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BO.BlNotFoundException($"Customer ID {customer.IDNumber} not found", ex);
        }
        catch (Exception ex) when (ex is not BO.BlNotFoundException)
        {
            throw new BO.BlInvalidInputException("Failed to update customer", ex);
        }
    }

    public void Delete(int id)
    {
        try
        {
            if (_dal.Customer.Read(id) == null)
                throw new BO.BlNotFoundException($"Customer ID {id} not found");
            _dal.Customer.Delete(id);
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BO.BlNotFoundException($"Customer ID {id} not found", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlInvalidInputException("Failed to delete customer", ex);
        }
    }
}