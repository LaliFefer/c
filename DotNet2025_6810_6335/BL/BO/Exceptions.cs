namespace BO;

[Serializable]
public class BlNotFoundException : Exception
{
    public BlNotFoundException(string? message) : base(message) { }
    public BlNotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlInvalidInputException : Exception
{
    public BlInvalidInputException(string? message) : base(message) { }
    public BlInvalidInputException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlNotEnoughInStockException : Exception
{
    public BlNotEnoughInStockException(string? message) : base(message) { }
    public BlNotEnoughInStockException(string message, Exception innerException)
        : base(message, innerException) { }
}
