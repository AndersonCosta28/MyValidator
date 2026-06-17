using System.Collections;

namespace Mert1s.MyValidator;

internal sealed class AsyncValidationRule<TInst, TProp> : IValidationRule<TInst>
{
    private readonly Func<TInst, TProp> _propertySelector;
    private readonly Func<TProp, TInst, CancellationToken, Task<bool>>? _conditionAsync;

    public Func<TProp, TInst, string> ErrorMessageFunc { get; set; } = default!;
    public Func<TProp, TInst, CancellationToken, Task<string>>? ErrorMessageFuncAsync { get; set; }
    public INestedValidator NestedValidator { get; set; } = default!;
    public string PathName { get; }
    public CascadeMode? CascadeMode { get; set; }
    public Func<TInst, bool>? When { get; set; }
    public Func<TInst, CancellationToken, Task<bool>>? WhenAsync { get; set; }

    public AsyncValidationRule(Expression<Func<TInst, TProp>> propertySelector, Expression<Func<TProp, CancellationToken, Task<bool>>> conditionAsync)
    {
        this.PathName = ValidationRule<TInst, TProp>.GetPropertyName(propertySelector);
        this._propertySelector = propertySelector.Compile();
        var compiled = conditionAsync.Compile();
        this._conditionAsync = (prop, _, ct) => compiled(prop, ct);
    }

    public AsyncValidationRule(Expression<Func<TInst, TProp>> propertySelector, Expression<Func<TProp, TInst, CancellationToken, Task<bool>>> conditionWithInstanceAsync)
    {
        this.PathName = ValidationRule<TInst, TProp>.GetPropertyName(propertySelector);
        this._propertySelector = propertySelector.Compile();
        this._conditionAsync = conditionWithInstanceAsync.Compile();
    }

    public AsyncValidationRule(Expression<Func<TInst, TProp>> propertySelector, Func<TProp, CancellationToken, Task<bool>> conditionAsync)
    {
        this.PathName = ValidationRule<TInst, TProp>.GetPropertyName(propertySelector);
        this._propertySelector = propertySelector.Compile();
        this._conditionAsync = (prop, _, ct) => conditionAsync(prop, ct);
    }

    public AsyncValidationRule(Expression<Func<TInst, TProp>> propertySelector, Func<TProp, TInst, CancellationToken, Task<bool>> conditionWithInstanceAsync)
    {
        this.PathName = ValidationRule<TInst, TProp>.GetPropertyName(propertySelector);
        this._propertySelector = propertySelector.Compile();
        this._conditionAsync = conditionWithInstanceAsync;
    }

    public string GetErrorMessage(TInst instance)
    {
        var property = this._propertySelector(instance);
        if (this.ErrorMessageFunc == null)
            return "Erro de validação.";
        var msg = this.ErrorMessageFunc.Invoke(property, instance);
        return msg ?? "Erro de validação.";
    }

    public async Task<string> GetErrorMessageAsync(TInst instance, CancellationToken cancellation = default)
    {
        if (this.ErrorMessageFuncAsync != null)
        {
            var property = this._propertySelector(instance);
            var msg = await this.ErrorMessageFuncAsync.Invoke(property, instance, cancellation).ConfigureAwait(false);
            return msg ?? "Erro de validação.";
        }
        return this.GetErrorMessage(instance);
    }

    public ValidationResult Validate(TInst instance)
    {
        var result = new ValidationResult();
        var value = this._propertySelector(instance);

        if (this.When != null && !this.When(instance))
            return result;

        if (this.WhenAsync != null)
        {
            var shouldRun = this.WhenAsync.Invoke(instance, CancellationToken.None).GetAwaiter().GetResult();
            if (!shouldRun)
                return result;
        }

        var ok = true;
        if (this._conditionAsync != null)
            ok = this._conditionAsync(value, instance, CancellationToken.None).GetAwaiter().GetResult();

        if (!ok)
            result.AddError(this.PathName, this.GetErrorMessageAsync(instance, CancellationToken.None).GetAwaiter().GetResult());

        if (this.NestedValidator != null && value != null)
        {
            if (value is IEnumerable<object> list)
            {
                var i = 0;
                foreach (var item in list)
                {
                    var nestedResult = this.NestedValidator.Validate(item);
                    result.Merge(this.PathName, $"{this.PathName}[{i}]", nestedResult);
                    i++;
                }
            }
            else
            {
                var nestedResult = this.NestedValidator.Validate(value!);
                result.Merge(this.PathName, this.PathName, nestedResult);
            }
        }

        return result;
    }

    public async Task<ValidationResult> ValidateAsync(TInst instance, CancellationToken cancellation = default)
    {
        var result = new ValidationResult();
        var value = this._propertySelector(instance);

        if (this.When != null && !this.When(instance))
            return result;

        if (this.WhenAsync != null)
        {
            var shouldRun = await this.WhenAsync.Invoke(instance, cancellation).ConfigureAwait(false);
            if (!shouldRun)
                return result;
        }

        var ok = true;
        if (this._conditionAsync != null)
            ok = await this._conditionAsync.Invoke(value, instance, cancellation).ConfigureAwait(false);

        if (!ok)
            result.AddError(this.PathName, await this.GetErrorMessageAsync(instance, cancellation).ConfigureAwait(false));

        if (this.NestedValidator != null && value != null)
        {
            if (value is IEnumerable<object> list)
            {
                var i = 0;
                foreach (var item in list)
                {
                    var nestedResults = await this.NestedValidator.ValidateAsync(item, cancellation).ConfigureAwait(false);
                    result.Merge(this.PathName, $"{this.PathName}[{i}]", nestedResults);
                    i++;
                }
            }
            else
            {
                var nestedResult = await this.NestedValidator.ValidateAsync(value!, cancellation).ConfigureAwait(false);
                result.Merge(this.PathName, this.PathName, nestedResult);
            }
        }

        return result;
    }
}
