namespace BO;

public class Sale
{
    public int IDNumber { get; init; }
    public int ProductIDNumber { get; set; }
    public int QuantityItemsRequiredtoReceivetheSale { get; set; }
    public double FullPrice { get; set; }
    public bool SaleOnlyforClubCustomers { get; set; }
    public string SaleStartDate { get; set; } = string.Empty;
    public string SaleEndDate { get; set; } = string.Empty;

    public override string ToString() => this.ToStringProperty();
}