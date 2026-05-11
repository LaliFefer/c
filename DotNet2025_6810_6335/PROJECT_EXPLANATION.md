# הסבר על פרויקט הקופה

## רקע כללי
הפרויקט הוא מערכת קופה רושמת בשפה C# ב-Visual Studio. המערכת בנויה על ארכיטקטורת שכבות:
- DAL (Data Access Layer) - שכבת גישה לנתונים
- BL (Business Layer) - שכבת לוגיקה עסקית
- UI / BlTest - שכבת הממשק למשתמש

נושא הפרויקט: חנות קמעונאית עם מוצרים, לקוחות ומבצעים.

---

## מבנה הפרויקט הנוכחי

### DalFacede
זה הפרויקט שמכיל את החוזים וההגדרות של DAL.
- `DalApi/ICrud.cs` - ממשק גנרי CRUD עם מתודות `Create`, `Read`, `ReadAll`, `Update`, `Delete`.
- `DalApi/IProduct.cs`, `ICustomer.cs`, `ISale.cs` - ממשקי CRUD לכל ישות.
- `DalApi/IDal.cs` - ממשק מרכזי שמכיל תכונות `Product`, `Customer`, `Sale` שמחזירות תתי-ממשקים.
- `DalApi/Factory.cs` - יוצר מופע של DAL בפועל על פי הגדרות קובץ `dal-config.xml`.
- `DO/` - ישויות נתונים (Data Objects): `Product`, `Customer`, `Sale` ו-`Categories`.

המשמעות: כל השכבה שמעל יכולה לעבוד עם `DalApi.IDal` ועם הישויות `DO`, בלי לדעת מי מיישם את הנתונים בפועל.

### DalList
זה הפרויקט שמיישם את DAL בעזרת רשימות זיכרון (`List<T>`).
- `DalList/Class1.cs` - מספק singleton של `DalList` שמיישם `DalApi.IDal`.
- `DalList/ProductImplementation.cs` - מימוש CRUD עבור מוצרים באמצעות רשימות.
- `DalList/CustomerImplementation.cs` - מימוש CRUD עבור לקוחות.
- `DalList/SaleImplementation.cs` - מימוש CRUD עבור מבצעים.
- הנתונים מבוססים על `DataSource` סטטי (ברשימות), כלומר כל הריצה מתחילה מחדש עם אותם נתונים.

### BL
השכבה העסקית של המערכת.
- `BL/BO/` - ישויות לוגיות של BL.
  - `Customer.cs`, `Product.cs`, `Sale.cs` - עותקים של הישויות מה-DAL עם התאמות BL.
  - `Order.cs` - ישות לוגית לתיאור הזמנה של קופה.
  - `ProductInOrder.cs` - תיאור מוצר בתוך הזמנה.
  - `SaleInProduct.cs` - תיאור מבצע שרלוונטי למוצר בהזמנה.
  - `Exceptions.cs` - חריגות BL מותאמות.
  - `Tools.cs` - מתודות עזר ל-BL: `ToStringProperty` + המרות בין DO ל-BO ולהיפך.
- `BL/BlApi/` - ממשקי BL.
  - `ICustomer`, `IProduct`, `ISale`, `IOrder` - ממשקי שירות BL לכל ישות.
  - `IBl` - ממשק ראשי שמרכז את כל תתי הממשקים.
  - `Factory.cs` - יוצר מופע של `IBl`.
- `BL/BlImplementation/` - מימוש ממשקי BL.
  - `CustomerImplementation.cs` - קורא ל-DAL וממיר ל-BL.
  - `ProductImplementation.cs` - מציג את רשימת המוצרים, קבלת מוצר, הוספה, עדכון ומחיקה.
  - `SaleImplementation.cs` - מייצר את רשימת המבצעים והשימוש ב-CRUD.
  - `OrderImplementation.cs` - לוגיקת הזמנות מלאה.
  - `Bl.cs` - מחלקה שמיישמת `IBl` ומחזירה מופעי תתי-ממשקים.

### BlTest
- פרויקט קונסול לבדיקות של השכבה העסקית.
- כולל תפריט ידני שמפעיל פעולות BL כמו:
  - הצגת מוצרים
  - הצגת מבצעים
  - חיפוש לקוח
  - הוספת מוצר
  - עדכון מוצר
  - יצירת הזמנה ופעולת `DoOrder`

---

## איך השכבות מתקשרות זו עם זו

### 1. DAL ל-BL
ה-BL לא שומר נתונים בעצמו. הוא לוקח מידע מה-DAL ומשנה אותו לישויות לוגיות (BO).
- `CustomerImplementation` קורא ל-`_dal.Customer.Read(id)` וממיר ל-`BO.Customer`.
- `ProductImplementation` קורא ל-`_dal.Product.ReadAll()` וממיר ל-`BO.Product`.
- `SaleImplementation` קורא ל-`_dal.Sale.ReadAll()` וממיר ל-`BO.Sale`.

### 2. BL ל-DAL
כאשר BL מקבל ישות מה-UI, הוא ממיר אותה ל-DO ושולח ל-DAL.
- בדוגמה של הוספת לקוח: `BO.Customer` מומר ל-`DO.Customer` ואז `Create` ב-DAL.

### 3. UI ל-BL
הפרויקט `BlTest` מדגים איך ה-UI יכול לקרוא ל-`BlApi.Factory.Get()` ולקבל `IBl`.
כל פעולה עוברת דרך ה-BL, וה-BL מטפל ב"לוגיקה העסקית".

---

## מה כוללת הלוגיקה העסקית (BL)

### מבנה הזמנה
- `BO.Order` - מכיל:
  - `IsClubCustomer` - האם הלקוח הוא מועדון.
  - `Products` - רשימת מוצרים בהזמנה.
  - `TotalPrice` - הסכום הסופי.

- `BO.ProductInOrder` - מכיל:
  - `ProductID`, `ProductName`, `BasePrice`, `Quantity`
  - `Sales` - רשימת מבצעים שנמצאו עבור אותו מוצר.
  - `TotalPrice` - המחיר הסופי למוצר.

- `BO.SaleInProduct` - מכיל:
  - `IDNumber`, `QuantityForSale`, `Price`, `ForAllCustomers`

### לוגיקת הזמנה
- `SearchSaleForProduct` - בודק מבצעים תקפים לפי תאריכים, כמות מוצר והאם הלקוח מועדון.
- `CalcTotalPriceForProduct` - משתמש במבצעים כדי לחלק את הכמות ולהפחית ממחיר המוצר.
- `CalcTotalPrice` - מחבר את מחירי המוצרים להזמנה.
- `AddProductToOrder` - מוסיף מוצר להזמנה, בודק מלאי, מחשב מבצעים ומעדכן מחיר.
- `DoOrder` - מבצע את ההזמנה בפועל: מוריד מהמלאי ב-DAL.

---

## נקודות חשובות להסבר

### הפרדה בין DO ל-BO
- DO = ישויות משתמשות ב-DAL, מבנה המידע הגולמי שמאוחסן.
- BO = ישויות BL, מבנה מידע שמותאם לעבודה עם הלוגיקה ולממשק המשתמש.
- שני המרחבים יכולים להיות דומים במבנה, אבל השכבות מופרדות כדי למנוע תלות ישרה.

### singleton ו-Factory
- ב-DAL נעשה שימוש ב-`DalApi.Factory.Get` כדי לקבל את המימוש הנכון של DAL.
- ב-BL נעשה שימוש ב-`BlApi.Factory.Get` כדי לקבל `IBl` בלי לדעת מה המימוש הפנימי.

### מטרת BL
- לאחסן **חוקים עסקיים** בלבד.
- לא לחפש או לשמור נתונים בעצמו.
- להמיר בין ישויות BL לישויות DAL ולשמור על עקביות ותוקף.

### מצב בנייה
- כרגע `DalFacede`, `DalList`, `BL` ו-`BlTest` מותאמים ל-`.NET 9.0`.
- הקומפילציה עובדת והפרויקט בונה בהצלחה.

---

## מה ניתן להסביר על הקוד בפירוט

### דוגמה לזרימת קריאה
1. `BlTest` קורא ל-`BlApi.Factory.Get()` ויוצר `IBl`.
2. הקריאה עוברת ל-`BlImplementation.Bl`.
3. `Bl` מחזיר את `ProductImplementation`, `CustomerImplementation`, `SaleImplementation` או `OrderImplementation`.
4. `ProductImplementation` קורא ל-`DalApi.Factory.Get` כדי לקבל `IDal` ממשי.
5. `IDal` מצביע ל-`DalList` (שכן `dal-config.xml` מוגדר לבחירת `list`).
6. `DalList` מבצע את פעולת ה-CRUD בזיכרון.
7. התוצאה מועברת חזרה דרך BL למשתמש.

### מה הקוד לא עושה כרגע
- אין ממשק גרפי מלא (Windows Forms) בפרויקט הנוכחי.
- אין מימוש של `DalXml` בשלב 9.
- הנתונים לא נשמרים בקבצים; `DalList` משתמש בזיכרון בלבד.

---

## המלצה להסבר בפני אחרים
כאשר אתה מסביר את הפרויקט, תוכל לחלק את זה ל:
1. "מה זה עושה" - מערכת חנות עם מוצרים, לקוחות, מבצעים והזמנות.
2. "איך זה מאורגן" - שלוש שכבות עיקריות: DAL, BL, UI.
3. "איך הזרימה עובדת" - UI -> BL -> DAL -> נתונים, וחזרה.
4. "למה יש BO ו-DO" - הפרדה בין נתוני אחסון לבין לוגיקה עסקית.
5. "מה הושג" - שלב 8 מלא, כולל Order ו-BL מתפקד.

---

## קבצים חשובים להסבר
- `BL/BlApi/IBl.cs`
- `BL/BlApi/IOrder.cs`
- `BL/BlImplementation/OrderImplementation.cs`
- `BL/BO/Order.cs`, `ProductInOrder.cs`, `SaleInProduct.cs`
- `DalFacede/DalApi/IDal.cs`
- `DalFacede/DalApi/ICrud.cs`
- `DalList/Class1.cs`
- `DalTest/Program.cs`

בהצלחה בהצגה! אם תרצה, אני יכול גם להכין גרסה של `PROJECT_EXPLANATION.md` עם דיאגרמות או נקודות שיחה מסודרות להצגה בפני מורֶה.