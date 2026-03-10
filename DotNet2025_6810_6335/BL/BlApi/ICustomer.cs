namespace BlApi;

public interface ICustomer
{
    // קבלת פרטי לקוח (BO) לפי תעודת זהות
    BO.Customer GetById(int id);

    // הוספת לקוח חדש
    void Add(BO.Customer customer);

    // עדכון פרטי לקוח
    void Update(BO.Customer customer);
}