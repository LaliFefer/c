namespace BlApi;

public interface ISale
{
    // מחזיר את כל המבצעים הקיימים
    IEnumerable<BO.Sale> GetList();

    // מחזיר מבצע ספציפי לפי קוד
    BO.Sale GetById(int id);

    // הוספת מבצע חדש
    void Add(BO.Sale sale);

    // עדכון מבצע קיים
    void Update(BO.Sale sale);

    // מחיקת מבצע
    void Delete(int id);
}