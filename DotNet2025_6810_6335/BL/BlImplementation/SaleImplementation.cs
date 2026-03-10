namespace BlImplementation;
using BlApi;
using BO;

internal class SaleImplementation : ISale
{
    private DalApi.IDal _dal = DalApi.Factory.Get();

    public IEnumerable<BO.Sale> GetList()
    {
        return from doSale in _dal.Sale.GetList()
               select new BO.Sale
               {
                   IDNumber = doSale.IDNumber,
                   ProductIDNumber = doSale.ProductIDNumber,
                   QuantityItemsRequiredtoReceivetheSale = doSale.QuantityItemsRequiredtoReceivetheSale,
                   FullPrice = doSale.FullPrice,
                   SaleOnlyforClubCustomers = doSale.SaleOnlyforClubCustomers,
                   SaleStartDate = doSale.SaleStartDate,
                   SaleEndDate = doSale.SaleEndDate
               };
    }

    public BO.Sale GetById(int id)
    {
        try
        {
            var doSale = _dal.Sale.GetById(id);
            return new BO.Sale
            {
                IDNumber = doSale.IDNumber,
                ProductIDNumber = doSale.ProductIDNumber,
                QuantityItemsRequiredtoReceivetheSale = doSale.QuantityItemsRequiredtoReceivetheSale,
                FullPrice = doSale.FullPrice,
                SaleOnlyforClubCustomers = doSale.SaleOnlyforClubCustomers,
                SaleStartDate = doSale.SaleStartDate,
                SaleEndDate = doSale.SaleEndDate
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Sale with ID {id} not found", ex);
        }
    }

    public void Add(BO.Sale sale)
    {
        try
        {
            _dal.Sale.Add(new DO.Sale
            (
                sale.IDNumber,
                sale.ProductIDNumber,
                sale.QuantityItemsRequiredtoReceivetheSale,
                sale.FullPrice,
                sale.SaleOnlyforClubCustomers,
                sale.SaleStartDate,
                sale.SaleEndDate
            ));
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to add sale", ex);
        }
    }

    public void Update(BO.Sale sale) { /* מימוש דומה ל-Add */ }
    public void Delete(int id) { _dal.Sale.Delete(id); }
}