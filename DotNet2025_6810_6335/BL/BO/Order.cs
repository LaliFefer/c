namespace BO;

public class Order
{
    public bool IsClubCustomer { get; init; }
    public IEnumerable<ProductInOrder>? Products { get; set; }
    public double TotalPrice { get; set; }

    public override string ToString() => this.ToStringProperty();
}
