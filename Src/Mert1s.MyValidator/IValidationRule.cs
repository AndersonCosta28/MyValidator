namespace Mert1s.MyValidator;

internal interface IValidationRule<TInstance>
{
    string PathName { get; }
    CascadeMode? CascadeMode { get; set; }
    string GetErrorMessage(TInstance instance);
    ValidationResult Validate(TInstance instance);
    Task<ValidationResult> ValidateAsync(TInstance instance, CancellationToken cancellation = default);
}
