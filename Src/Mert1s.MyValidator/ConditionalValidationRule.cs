namespace Mert1s.MyValidator;

internal sealed class ConditionalValidationRule<TInstance> : IValidationRule<TInstance>
{
    private readonly Func<TInstance, bool> _condition;
    private readonly IValidationRule<TInstance> _inner;

    public ConditionalValidationRule(Func<TInstance, bool> condition, IValidationRule<TInstance> inner)
    {
        _condition = condition ?? throw new ArgumentNullException(nameof(condition));
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
        if (!_condition(instance))
            return new ValidationResult();

        return _inner.Validate(instance);
    }

    public Task<ValidationResult> ValidateAsync(TInstance instance, CancellationToken cancellation = default)
    {
        if (!_condition(instance))
            return Task.FromResult(new ValidationResult());

        return _inner.ValidateAsync(instance, cancellation);
    }
}
