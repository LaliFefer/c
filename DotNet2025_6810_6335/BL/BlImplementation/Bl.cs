namespace BlImplementation;
using BlApi;

internal class Bl : IBl
{
    // יצירת המופעים של מחלקות המימוש (כמו שכתבנו ב-IBl)
    public IProduct Product => new ProductImplementation();
    public ICustomer Customer => new CustomerImplementation();
    public ISale Sale => new SaleImplementation();
    public IOrder Order => new OrderImplementation();
}