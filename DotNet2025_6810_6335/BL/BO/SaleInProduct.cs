namespace BO;

public class SaleInProduct
{
    public int IDNumber { get; set; }
    public string SaleDescription { get; set; } = string.Empty;
    public double FinalPrice { get; set; }

    public override string ToString() => this.ToStringProperty();
}