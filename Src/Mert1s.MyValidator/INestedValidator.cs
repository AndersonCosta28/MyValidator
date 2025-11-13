namespace Mert1s.MyValidator;
internal interface INestedValidator
{
    internal List<ValidationResult> Validate(object instance);
    internal Task<List<ValidationResult>> ValidateAsync(object instance, CancellationToken cancellation = default);
}
