namespace Mert1s.MyValidator;

internal interface IValidationRule<TInstance>
{
    string PathName { get; }
    CascadeMode? CascadeMode { get; set; }

    // Optional synchronous predicate that controls whether the rule should run.
    Func<TInstance, bool>? When { get; set; }

    // Optional asynchronous predicate that controls whether the rule should run.
    Func<TInstance, CancellationToken, Task<bool>>? WhenAsync { get; set; }

    string GetErrorMessage(TInstance instance);
    ValidationResult Validate(TInstance instance);
    Task<ValidationResult> ValidateAsync(TInstance instance, CancellationToken cancellation = default);
}
