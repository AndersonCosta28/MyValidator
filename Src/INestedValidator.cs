namespace MyValidator;
internal interface INestedValidator
{
    List<ValidationResult> ValidateWithResult(object instance);
}
