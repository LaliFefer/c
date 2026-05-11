namespace DO;

/// <summary>
/// Sale: data record for sales representing a sale transaction.
/// </summary>
/// <param name="IDNumber">מזהה ייחודי של העסקה.</param>
/// <param name="ProductIDNumber">מזהה המוצר שנמכר בעסקה.</param>
/// <param name="QuantityItemsRequiredtoReceivetheSale">כמות הפריטים שנמכרים בעסקה.</param>
/// <param name="FullPrice">סכום התשלום הכולל של העסקה.</param>
/// <param name="SaleOnlyforClubCustomers">מציין אם העסקה מיועדת רק ללקוחות מועדון.</param>
/// <param name="SaleStartDate">תאריך התחלה של העסקה.</param>
/// <param name="SaleEndDate">תאריך סיום של העסקה.</param>
public record Sale(
    int IDNumber,
    int ProductIDNumber,
    int QuantityItemsRequiredtoReceivetheSale,
    double FullPrice,
    bool SaleOnlyforClubCustomers,
    string SaleStartDate,
    string SaleEndDate
)
{
    // קונסטרקטור פרמטרלס ליצירת רשומה ריקה בעת הצורך.
    public Sale() : this(0, 0, 0, 0.0, false, string.Empty, string.Empty) { }
}