namespace BO;

public class SaleInProduct
{
    public int IDNumber { get; init; }
    public int QuantityForSale { get; init; }
    public double Price { get; init; }
    public bool ForAllCustomers { get; init; }

    public override string ToString() => this.ToStringProperty();
}