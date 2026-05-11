namespace BO;

public class ProductInOrder
{
    public int ProductID { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public double BasePrice { get; init; }
    public int Quantity { get; set; }
    public IEnumerable<SaleInProduct>? Sales { get; set; }
    public double TotalPrice { get; set; }

    public override string ToString() => this.ToStringProperty();
}
