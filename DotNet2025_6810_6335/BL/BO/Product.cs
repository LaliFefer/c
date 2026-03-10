namespace BO;

public class Product
{
    public int IDNumber { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Categories Category { get; set; }
    public double Price { get; set; }
    public int QuantityInStock { get; set; }

    // שימו לב: זו תכונה חדשה שקיימת רק ב-BL (אוסף של מבצעים למוצר)
    public IEnumerable<BO.SaleInProduct>? Sales { get; set; }

    public override string ToString() => this.ToStringProperty();
}