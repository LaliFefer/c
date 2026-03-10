using System.Reflection;
using System.Linq;

namespace BO;

internal static class Tools
{
    // מתודת הרחבה (Extension Method) שעוברת על כל המאפיינים של האובייקט
    public static string ToStringProperty<T>(this T obj)
    {
        if (obj == null) return "";

        string str = "";
        // מעבר על כל ה-Properties של המחלקה בעזרת Reflection
        foreach (PropertyInfo prop in obj.GetType().GetProperties())
        {
            var value = prop.GetValue(obj, null);

            // בדיקה אם המאפיין הוא רשימה (כמו רשימת המבצעים במוצר)
            if (value is System.Collections.IEnumerable list && !(value is string))
            {
                str += "\n" + prop.Name + ": " + string.Join(", ", list.Cast<object>());
            }
            else
            {
                str += "\n" + prop.Name + ": " + (value ?? "null");
            }
        }
        return str;
    }
}