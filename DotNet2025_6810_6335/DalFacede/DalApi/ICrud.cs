using System.Collections.Generic;

namespace DalApi
{
    // ICrud: generic CRUD interface
    public interface ICrud<T> where T : class
    {
        int Create(T item);
        T? Read(int id);
        List<T?> ReadAll(Func<T, bool>? filter = null); 
        T? Read(Func<T, bool> filter); 
        void Update(T item);
        void Delete(int id);
    }
}
