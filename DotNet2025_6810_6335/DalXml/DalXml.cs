using DalApi;

namespace Dal;

internal sealed class DalXml : DalApi.IDal
{
 // private read-only singleton instance (get-only auto-property)
 private static DalXml instance { get; } = new DalXml();

 // public accessor for external code expecting IDal
 public static DalApi.IDal Instance => instance;

 // private ctor to prevent external instantiation
 private DalXml()
 {
 }

 public IProduct Product => new ProductImplementation();
 public ICustomer Customer => new CustomerImplementation();
 public ISale Sale => new SaleImplementation();
}
