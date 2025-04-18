namespace MyValidator;

public class ValidationError
{
    public required string Path { get; set; }
    public required string Message { get; set; }
}
