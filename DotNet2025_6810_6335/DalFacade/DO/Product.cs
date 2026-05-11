namespace DO;

/// <summary>
/// Product: data record for products representing an item in the store.
/// </summary>
/// <param name="IDNumber">מזהה ייחודי של המוצר.</param>
/// <param name="ProductName">שם המוצר.</param>
/// <param name="Category">קטגוריית המוצר מתוך Enums.Categories.</param>
/// <param name="Price">מחיר יחידה של המוצר.</param>
/// <param name="QuantityInStock">כמות המוצר במלאי.</param>
public record Product(
    int IDNumber,
    string ProductName,
    Categories Category,
    double Price,
    int QuantityInStock
)
{
    // קונסטרקטור פרמטרלס נדרש ל־XmlSerializer.
    public Product() : this(0, string.Empty, Categories.MEN, 0.0, 0) { }
}
