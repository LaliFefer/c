namespace BO;

public class Customer
{
    public int IDNumber { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string TelephoneNumber { get; set; } = string.Empty;

    public override string ToString() => this.ToStringProperty();
}