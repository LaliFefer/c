namespace DO;

/// <summary>
/// Customer: data record for customers representing a customer entity.
/// </summary>
/// <param name="IDNumber">מספר זהות ייחודי של הלקוח.</param>
/// <param name="CustomerName">שם הלקוח.</param>
/// <param name="EmailAddress">כתובת דוא"ל של הלקוח.</param>
/// <param name="TelephoneNumber">מספר טלפון של הלקוח.</param>
/// <param name="IsClubMember">האם הלקוח שייך למועדון.</param>
public record class Customer(
    int IDNumber,
    string CustomerName,
    string EmailAddress,
    string TelephoneNumber,
    bool IsClubMember = false)
{
    // קונסטרקטור פרמטרלס נדרש במקרים של XML serializer או יצירת אובייקט ריק.
    public Customer() : this(0, string.Empty, string.Empty, string.Empty, false) { }
}





