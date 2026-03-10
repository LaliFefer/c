namespace BlImplementation;
using BlApi;
using BO;

internal class CustomerImplementation : ICustomer
{
    private DalApi.IDal _dal = DalApi.Factory.Get();

    public BO.Customer GetById(int id)
    {
        try
        {
            var doCust = _dal.Customer.GetById(id);
            return new BO.Customer
            {
                IDNumber = doCust.IDNumber,
                CustomerName = doCust.CustomerName,
                EmailAddress = doCust.EmailAddress,
                TelephoneNumber = doCust.TelephoneNumber
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Customer with ID {id} not found", ex);
        }
    }

    public void Add(BO.Customer customer)
    {
        // כאן אפשר להוסיף לוגיקה: למשל לבדוק שה-ID חיובי
        if (customer.IDNumber <= 0)
            throw new Exception("Invalid ID Number");

        try
        {
            // המרה חזרה מ-BO ל-DO כדי לשמור בדאטה בייס
            _dal.Customer.Add(new DO.Customer
            (
                customer.IDNumber,
                customer.CustomerName,
                customer.EmailAddress,
                customer.TelephoneNumber
            ));
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to add customer", ex);
        }
    }

    public void Update(BO.Customer customer)
    {
        try
        {
            _dal.Customer.Update(new DO.Customer
            (
                customer.IDNumber,
                customer.CustomerName,
                customer.EmailAddress,
                customer.TelephoneNumber
            ));
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to update customer", ex);
        }
    }
}