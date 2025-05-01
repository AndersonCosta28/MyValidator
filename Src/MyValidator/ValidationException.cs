namespace MyValidator;

public class ValidationException : Exception
{
    public List<ValidationError> ValidationErrors { get; private set; } = [];

    public ValidationException(List<ValidationError> validationErrors)
    {
        this.ValidationErrors = validationErrors;
    }

    public ValidationException(List<ValidationError> validationErrors, string message) : base(message)
    {
        this.ValidationErrors = validationErrors;
    }

    public ValidationException(List<ValidationError> validationErrors, string message, Exception innerException) : base(message, innerException)
    {
        this.ValidationErrors = validationErrors;
    }
}