namespace DalList
{
    // DalList: factory that returns DAL implementations
    public class DalList : DalApi.IDal
    {
        // כל תכונה מחזירה מופע של מימוש המתאים בתוך פרויקט DalList
        public DalApi.IProduct Product => new Dal.ProductImplementation();

        public DalApi.ICustomer Customer => new Dal.CustomerImplementation();

        public DalApi.ISale Sale => new Dal.SaleImplementation();
    }
}

