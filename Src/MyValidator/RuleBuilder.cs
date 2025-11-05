namespace Mert1s.MyValidator;

public class RuleBuilder<TInstance, TProperty>
{
    private readonly Expression<Func<TInstance, TProperty>> _propertySelector;
    private readonly List<IValidationRule<TInstance>> _rules;
    private readonly List<IValidationRule<TInstance>> _internalRules = [];
    private IValidationRule<TInstance> _currentRule = null!;

    internal RuleBuilder(Expression<Func<TInstance, TProperty>> propertySelector, List<IValidationRule<TInstance>> rules)
    {
        this._propertySelector = propertySelector;
        this._currentRule = null!;
        this._rules = rules;
    }

    public RuleBuilder<TInstance, TProperty> Must(Expression<Func<TProperty, bool>> condition)
    {
        var rule = new ValidationRule<TInstance, TProperty>(this._propertySelector, condition);
        this._currentRule = rule;
        this._rules.Add(rule);
        this._internalRules.Add(rule);
        return this;
    }

    public RuleBuilder<TInstance, TProperty> Must(Expression<Func<TProperty, TInstance, bool>> condition)
    {
        var rule = new ValidationRule<TInstance, TProperty>(this._propertySelector, condition);
        this._currentRule = rule;
        this._rules.Add(rule);
        this._internalRules.Add(rule);
        return this;
    }

    internal RuleBuilder<TInstance, TProperty> Must(Expression<Func<TProperty, bool>> condition, Expression<Func<TProperty, TInstance, string>> func)
    {
        var rule = new ValidationRule<TInstance, TProperty>(this._propertySelector, condition)
        {
            ErrorMessageFunc = (property, instance) => func.Compile().Invoke(property, instance)
        };
        this._currentRule = rule;
        this._rules.Add(rule);
        this._internalRules.Add(rule);
        return this;
    }

    internal RuleBuilder<TInstance, TProperty> Must(Expression<Func<TProperty, TInstance, bool>> condition, Expression<Func<TProperty, TInstance, string>> func)
    {
        var rule = new ValidationRule<TInstance, TProperty>(this._propertySelector, condition)
        {
            ErrorMessageFunc = (property, instance) => func.Compile().Invoke(property, instance)
        };
        this._currentRule = rule;
        this._rules.Add(rule);
        this._internalRules.Add(rule);
        return this;
    }

    public RuleBuilder<TInstance, TProperty> Message(string message)
    {
        foreach (var rule in this._internalRules)
        {
            if (rule is ValidationRule<TInstance, TProperty> validationRule && validationRule.ErrorMessageFunc == null)
            {
                validationRule.ErrorMessageFunc = (_, _) => message;
                break;
            }

            if (rule is AsyncValidationRule<TInstance, TProperty> asyncRule && asyncRule.ErrorMessageFunc == null)
            {
                asyncRule.ErrorMessageFunc = (_, _) => message;
                break;
            }
        }

        if (this._currentRule is ValidationRule<TInstance, TProperty> vr)
            vr.ErrorMessageFunc = (_, _) => message;
        else if (this._currentRule is AsyncValidationRule<TInstance, TProperty> ar)
            ar.ErrorMessageFunc = (_, _) => message;

        return this;
    }

    public RuleBuilder<TInstance, TProperty> Message(Expression<Func<TProperty, TInstance, string>> func)
    {
        foreach (var rule in this._internalRules.AsEnumerable().Reverse())
        {
            if (rule is ValidationRule<TInstance, TProperty> validationRule && validationRule.ErrorMessageFunc == null)
            {
                validationRule.ErrorMessageFunc = (property, instance) => func.Compile().Invoke(property, instance);
                break;
            }

            if (rule is AsyncValidationRule<TInstance, TProperty> asyncRule && asyncRule.ErrorMessageFunc == null)
            {
                asyncRule.ErrorMessageFunc = (property, instance) => func.Compile().Invoke(property, instance);
                break;
            }
        }

        if (this._currentRule is ValidationRule<TInstance, TProperty> vr)
            vr.ErrorMessageFunc = (property, instance) => func.Compile().Invoke(property, instance);
        else if (this._currentRule is AsyncValidationRule<TInstance, TProperty> ar)
            ar.ErrorMessageFunc = (property, instance) => func.Compile().Invoke(property, instance);

        return this;
    }

    public RuleBuilder<TInstance, TProperty> Message(Expression<Func<TProperty, string>> func)
    {
        foreach (var rule in this._internalRules)
        {
            if (rule is ValidationRule<TInstance, TProperty> validationRule && validationRule.ErrorMessageFunc == null)
            {
                validationRule.ErrorMessageFunc = (property, instance) => func.Compile().Invoke(property);
                break;
            }

            if (rule is AsyncValidationRule<TInstance, TProperty> asyncRule && asyncRule.ErrorMessageFunc == null)
            {
                asyncRule.ErrorMessageFunc = (property, instance) => func.Compile().Invoke(property);
                break;
            }
        }

        if (this._currentRule is ValidationRule<TInstance, TProperty> vr)
            vr.ErrorMessageFunc = (property, instance) => func.Compile().Invoke(property);
        else if (this._currentRule is AsyncValidationRule<TInstance, TProperty> ar)
            ar.ErrorMessageFunc = (property, instance) => func.Compile().Invoke(property);

        return this;
    }

    public RuleBuilder<TInstance, TProperty> Message(Expression<Func<string>> func)
    {
        foreach (var rule in this._internalRules)
        {
            if (rule is ValidationRule<TInstance, TProperty> validationRule && validationRule.ErrorMessageFunc == null)
            {
                validationRule.ErrorMessageFunc = (property, instance) => func.Compile().Invoke();
                break;
            }

            if (rule is AsyncValidationRule<TInstance, TProperty> asyncRule && asyncRule.ErrorMessageFunc == null)
            {
                asyncRule.ErrorMessageFunc = (property, instance) => func.Compile().Invoke();
                break;
            }
        }

        if (this._currentRule is ValidationRule<TInstance, TProperty> vr)
            vr.ErrorMessageFunc = (property, instance) => func.Compile().Invoke();
        else if (this._currentRule is AsyncValidationRule<TInstance, TProperty> ar)
            ar.ErrorMessageFunc = (property, instance) => func.Compile().Invoke();

        return this;
    }

    public RuleBuilder<TInstance, TProperty> SetValidator(ValidatorBuilder<TProperty> validator)
    {
        var rule = new ValidationRule<TInstance, TProperty>(this._propertySelector, validator);
        this._currentRule = rule;
        this._rules.Add(rule);
        return this;
    }

    public RuleBuilder<TInstance, TProperty> MustAsync(Expression<Func<TProperty, CancellationToken, Task<bool>>> condition)
    {
        var rule = new AsyncValidationRule<TInstance, TProperty>(this._propertySelector, condition);
        this._currentRule = rule;
        this._rules.Add(rule);
        this._internalRules.Add(rule);
        return this;
    }

    public RuleBuilder<TInstance, TProperty> MustAsync(Expression<Func<TProperty, TInstance, CancellationToken, Task<bool>>> condition)
    {
        var rule = new AsyncValidationRule<TInstance, TProperty>(this._propertySelector, condition);
        this._currentRule = rule;
        this._rules.Add(rule);
        this._internalRules.Add(rule);
        return this;
    }

    // New overloads that accept compiled delegates so tests can pass async lambdas
    public RuleBuilder<TInstance, TProperty> MustAsync(Func<TProperty, CancellationToken, Task<bool>> condition)
    {
        var rule = new AsyncValidationRule<TInstance, TProperty>(this._propertySelector, condition);
        this._currentRule = rule;
        this._rules.Add(rule);
        this._internalRules.Add(rule);
        return this;
    }

    public RuleBuilder<TInstance, TProperty> MustAsync(Func<TProperty, TInstance, CancellationToken, Task<bool>> condition)
    {
        var rule = new AsyncValidationRule<TInstance, TProperty>(this._propertySelector, condition);
        this._currentRule = rule;
        this._rules.Add(rule);
        this._internalRules.Add(rule);
        return this;
    }

    internal RuleBuilder<TInstance, TProperty> MustAsync(Expression<Func<TProperty, CancellationToken, Task<bool>>> condition, Expression<Func<TProperty, TInstance, string>> func)
    {
        var rule = new AsyncValidationRule<TInstance, TProperty>(this._propertySelector, condition)
        {
            ErrorMessageFunc = (property, instance) => func.Compile().Invoke(property, instance)
        };
        this._currentRule = rule;
        this._rules.Add(rule);
        this._internalRules.Add(rule);
        return this;
    }

    internal RuleBuilder<TInstance, TProperty> MustAsync(Expression<Func<TProperty, TInstance, CancellationToken, Task<bool>>> condition, Expression<Func<TProperty, TInstance, string>> func)
    {
        var rule = new AsyncValidationRule<TInstance, TProperty>(this._propertySelector, condition)
        {
            ErrorMessageFunc = (property, instance) => func.Compile().Invoke(property, instance)
        };
        this._currentRule = rule;
        this._rules.Add(rule);
        this._internalRules.Add(rule);
        return this;
    }

    public RuleBuilder<TInstance, TProperty> SetValidatorAsync(ValidatorBuilder<TProperty> validator)
    {
        // Nested validator already supports async ValidateAsync, so we can reuse ValidationRule for nested scenarios.
        var rule = new ValidationRule<TInstance, TProperty>(this._propertySelector, validator);
        this._currentRule = rule;
        this._rules.Add(rule);
        return this;
    }

    private sealed class AsyncValidationRule<TInst, TProp> : IValidationRule<TInst>
    {
        private readonly Func<TInst, TProp> _propertySelector;
        private readonly Func<TProp, TInst, CancellationToken, Task<bool>>? _conditionAsync;

        public Func<TProp, TInst, string> ErrorMessageFunc { get; set; } = default!;
        public INestedValidator NestedValidator { get; set; } = default!;
        public string PathName { get; }

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

        // New constructors that accept compiled delegates
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

        public ValidationResult Validate(TInst instance)
        {
            var result = new ValidationResult();
            var value = this._propertySelector(instance);

            var ok = true;
            if (this._conditionAsync != null)
            {
                ok = this._conditionAsync(value, instance, CancellationToken.None).GetAwaiter().GetResult();
            }

            if (!ok)
                result.AddError(this.PathName, this.GetErrorMessage(instance));

            if (this.NestedValidator != null && value != null)
            {
                // Reuse nested sync validation
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

            var ok = true;
            if (this._conditionAsync != null)
                ok = await this._conditionAsync.Invoke(value, instance, cancellation).ConfigureAwait(false);

            if (!ok)
                result.AddError(this.PathName, this.GetErrorMessage(instance));

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
}
