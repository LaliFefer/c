namespace DalApi;
using DO;

public interface ICustomer
{
    int Create(Customer item);
    Customer? Read(int id);
    List<Customer> ReadAll();
    void Update(Customer item);
    void Delete(int id);
}