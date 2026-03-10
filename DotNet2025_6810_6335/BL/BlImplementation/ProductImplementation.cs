namespace BlImplementation;
using BlApi;
using BO;

internal class ProductImplementation : IProduct
{
    // גישה לשכבת הנתונים
    private DalApi.IDal _dal = DalApi.Factory.Get();

    public IEnumerable<BO.Product> GetList()
    {
        // הופך את כל המוצרים מה-DAL למוצרים לוגיים של ה-BL
        return from doProd in _dal.Product.GetList()
               select new BO.Product
               {
                   IDNumber = doProd.IDNumber,
                   ProductName = doProd.ProductName,
                   Category = (BO.Categories)doProd.Category, // המרה של ה-Enum
                   Price = doProd.Price,
                   QuantityInStock = doProd.QuantityInStock
               };
    }

    public BO.Product GetById(int id)
    {
        try
        {
            var doProd = _dal.Product.GetById(id);
            return new BO.Product
            {
                IDNumber = doProd.IDNumber,
                ProductName = doProd.ProductName,
                Category = (BO.Categories)doProd.Category,
                Price = doProd.Price,
                QuantityInStock = doProd.QuantityInStock
            };
        }
        catch (Exception ex)
        {
            // אם המוצר לא נמצא בנתונים, אנחנו זורקים שגיאה לוגית
            throw new Exception("Product not found", ex);
        }
    }

    public void Add(BO.Product product) { /* נממש בהמשך */ }
    public void Update(BO.Product product) { /* נממש בהמשך */ }
    public void Delete(int id) { /* נממש בהמשך */ }
}