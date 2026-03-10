namespace BlApi;

public interface IProduct
{
    // מחזיר את כל המוצרים בפורמט לוגי
    IEnumerable<BO.Product> GetList();

    // מחזיר מוצר ספציפי לפי מספר זהות
    BO.Product GetById(int id);

    // הוספת מוצר חדש
    void Add(BO.Product product);

    // עדכון מוצר קיים
    void Update(BO.Product product);

    // מחיקת מוצר
    void Delete(int id);
}