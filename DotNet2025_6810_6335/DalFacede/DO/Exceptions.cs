namespace DO;

// קובץ זה מכיל חריגות כלליות של שכבת ה-DAL (Dal...)
// החריגות הן כלליות לכל הישויות ומיועדות להחליף "throw new Exception" בגישה מדויקת יותר

[Serializable]
public class DalEntityNotFoundException : Exception
{
 // בנאי המקבל הודעת שגיאה
 public DalEntityNotFoundException(string message) : base(message) { }
}

[Serializable]
public class DalEntityAlreadyExistsException : Exception
{
 // בנאי המקבל הודעת שגיאה
 public DalEntityAlreadyExistsException(string message) : base(message) { }
}
