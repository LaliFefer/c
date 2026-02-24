namespace DalList
{
    // DalList: factory that returns DAL implementations
    internal sealed class DalList : DalApi.IDal
    {
        // כל תכונה מחזירה מופע של מימוש המתאים בתוך פרויקט DalList
        public DalApi.IProduct Product => new Dal.ProductImplementation();

        public DalApi.ICustomer Customer => new Dal.CustomerImplementation();

        public DalApi.ISale Sale => new Dal.SaleImplementation();

        // private ctor for singleton
        private DalList() { }

        // single instance
        private static readonly DalList instance = new DalList();

        // public static Instance property for Factory to retrieve
        public static DalApi.IDal Instance => instance;
    }
}

