namespace LogCoreApi.Exceptions;

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; } 

    public ValidationException(IDictionary<string, string[]> errors, string message = "Validation failed")
        : base(message) 
    {
        Errors = errors; 
    }
}
