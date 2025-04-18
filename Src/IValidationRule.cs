namespace MyValidator;

internal interface IValidationRule<TInstance>
{
    string GetErrorMessage(TInstance instance);
    ValidationResult Validate(TInstance instance);
}
