namespace Mert1s.MyValidator;

internal sealed class ConditionalAsyncValidationRule<TInstance> : IValidationRule<TInstance>
{
    private readonly Func<TInstance, CancellationToken, Task<bool>> _conditionAsync;
    private readonly IValidationRule<TInstance> _inner;

    public ConditionalAsyncValidationRule(Func<TInstance, CancellationToken, Task<bool>> conditionAsync, IValidationRule<TInstance> inner)
    {
        _conditionAsync = conditionAsync ?? throw new ArgumentNullException(nameof(conditionAsync));
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public string PathName => _inner.PathName;

    public CascadeMode? CascadeMode
    {
        get => _inner.CascadeMode;
        set => _inner.CascadeMode = value;
    }

    public Func<TInstance, bool>? When
    {
        get => _inner.When;
        set => _inner.When = value;
    }

    public Func<TInstance, CancellationToken, Task<bool>>? WhenAsync
    {
        get => _inner.WhenAsync;
        set => _inner.WhenAsync = value;
    }

    public string GetErrorMessage(TInstance instance) => _inner.GetErrorMessage(instance);

    public ValidationResult Validate(TInstance instance)
    {
        // Execute async predicate synchronously for the sync path.
        var shouldRun = _conditionAsync(instance, CancellationToken.None).GetAwaiter().GetResult();
        if (!shouldRun)
            return new ValidationResult();

        return _inner.Validate(instance);
    }

    public async Task<ValidationResult> ValidateAsync(TInstance instance, CancellationToken cancellation = default)
    {
        var shouldRun = await _conditionAsync(instance, cancellation).ConfigureAwait(false);
        if (!shouldRun)
            return new ValidationResult();

        return await _inner.ValidateAsync(instance, cancellation).ConfigureAwait(false);
    }
}
