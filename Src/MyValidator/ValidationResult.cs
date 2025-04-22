namespace MyValidator;

public class ValidationResult
{
    public List<ValidationError> Errors { get; } = [];

    public bool IsValid => this.Errors.Count == 0;

    public void AddError(string path, string message) => this.Errors.Add(new ValidationError { PropertyName = path, Path = path, Message = message });

    internal void Merge(string property, string parentPath, ValidationResult other)
    {
        foreach (var err in other.Errors)
        {
            var fullPath = string.IsNullOrWhiteSpace(parentPath) ? err.Path : $"{parentPath}.{err.Path}";
            this.Errors.Add(new ValidationError { PropertyName = property, Path = fullPath, Message = err.Message });
        }
    }

    internal void Merge(string property, string parentPath, List<ValidationResult> others)
    {
        foreach (var other in others)
            this.Merge(property, parentPath, other);
    }
}
