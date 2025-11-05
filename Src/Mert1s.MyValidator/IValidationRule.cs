using System.Threading;

namespace Mert1s.MyValidator;

internal interface IValidationRule<TInstance>
{
    string GetErrorMessage(TInstance instance);
    ValidationResult Validate(TInstance instance);
    Task<ValidationResult> ValidateAsync(TInstance instance, CancellationToken cancellation = default);
}
