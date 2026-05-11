namespace BO;

public class Customer
{
    public int IDNumber { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string EmailAddress { get; init; } = string.Empty;
    public string TelephoneNumber { get; init; } = string.Empty;
    public bool IsClubMember { get; init; }

    public override string ToString() => this.ToStringProperty();
}