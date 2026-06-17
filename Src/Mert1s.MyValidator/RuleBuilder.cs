namespace Mert1s.MyValidator;

public class RuleBuilder<TInstance, TProperty>
{
    private readonly Expression<Func<TInstance, TProperty>> _propertySelector;
    private readonly List<IValidationRule<TInstance>> _rules;
    private readonly List<IValidationRule<TInstance>> _internalRules = [];
    private IValidationRule<TInstance> _currentRule = null!;
    private CascadeMode? _cascadeMode;

    private void ApplyCascade(IValidationRule<TInstance> rule) =>
        rule.CascadeMode = this._cascadeMode;

    internal RuleBuilder(Expression<Func<TInstance, TProperty>> propertySelector, List<IValidationRule<TInstance>> rules)
    {
        this._propertySelector = propertySelector;
        this._currentRule = null!;
        this._rules = rules;
    }

    public RuleBuilder<TInstance, TProperty> Must(Expression<Func<TProperty, bool>> condition)
    {
        var rule = new ValidationRule<TInstance, TProperty>(this._propertySelector, condition);
        this.ApplyCascade(rule);
        this._currentRule = rule;
        this._rules.Add(rule);
        this._internalRules.Add(rule);
        return this;
    }

    public RuleBuilder<TInstance, TProperty> Must(Expression<Func<TProperty, TInstance, bool>> condition)
    {
        var rule = new ValidationRule<TInstance, TProperty>(this._propertySelector, condition);
        this.ApplyCascade(rule);
        this._currentRule = rule;
        this._rules.Add(rule);
        this._internalRules.Add(rule);
        return this;
    }

    internal RuleBuilder<TInstance, TProperty> Must(Expression<Func<TProperty, bool>> condition, Expression<Func<TProperty, TInstance, string>> func)
    {
        var compiled = func.Compile();
        var rule = new ValidationRule<TInstance, TProperty>(this._propertySelector, condition)
        {
            ErrorMessageFunc = (property, instance) => compiled(property, instance)
        };
        this.ApplyCascade(rule);
        this._currentRule = rule;
        this._rules.Add(rule);
        this._internalRules.Add(rule);
        return this;
    }

    internal RuleBuilder<TInstance, TProperty> Must(Expression<Func<TProperty, TInstance, bool>> condition, Expression<Func<TProperty, TInstance, string>> func)
    {
        var compiled = func.Compile();
        var rule = new ValidationRule<TInstance, TProperty>(this._propertySelector, condition)
        {
            ErrorMessageFunc = (property, instance) => compiled(property, instance)
        };
        this.ApplyCascade(rule);
        this._currentRule = rule;
        this._rules.Add(rule);
        this._internalRules.Add(rule);
        return this;
    }

    // Sets the error message for the most recently defined rule (_currentRule).
    public RuleBuilder<TInstance, TProperty> Message(string message)
    {
        if (this._currentRule is ValidationRule<TInstance, TProperty> vr)
            vr.ErrorMessageFunc = (_, _) => message;
        else if (this._currentRule is AsyncValidationRule<TInstance, TProperty> ar)
            ar.ErrorMessageFunc = (_, _) => message;
        return this;
    }

    public RuleBuilder<TInstance, TProperty> Message(Expression<Func<TProperty, TInstance, string>> func)
    {
        var compiled = func.Compile();
        if (this._currentRule is ValidationRule<TInstance, TProperty> vr)
            vr.ErrorMessageFunc = (property, instance) => compiled(property, instance);
        else if (this._currentRule is AsyncValidationRule<TInstance, TProperty> ar)
            ar.ErrorMessageFunc = (property, instance) => compiled(property, instance);
        return this;
    }

    public RuleBuilder<TInstance, TProperty> Message(Expression<Func<TProperty, string>> func)
    {
        var compiled = func.Compile();
        if (this._currentRule is ValidationRule<TInstance, TProperty> vr)
            vr.ErrorMessageFunc = (property, _) => compiled(property);
        else if (this._currentRule is AsyncValidationRule<TInstance, TProperty> ar)
            ar.ErrorMessageFunc = (property, _) => compiled(property);
        return this;
    }

    public RuleBuilder<TInstance, TProperty> Message(Expression<Func<string>> func)
    {
        var compiled = func.Compile();
        if (this._currentRule is ValidationRule<TInstance, TProperty> vr)
            vr.ErrorMessageFunc = (_, _) => compiled();
        else if (this._currentRule is AsyncValidationRule<TInstance, TProperty> ar)
            ar.ErrorMessageFunc = (_, _) => compiled();
        return this;
    }

    // Sets the async error message for the most recently defined rule (_currentRule).
    public RuleBuilder<TInstance, TProperty> MessageAsync(Func<TProperty, TInstance, CancellationToken, Task<string>> func)
    {
        if (this._currentRule is ValidationRule<TInstance, TProperty> vr)
            vr.ErrorMessageFuncAsync = func;
        else if (this._currentRule is AsyncValidationRule<TInstance, TProperty> ar)
            ar.ErrorMessageFuncAsync = func;
        return this;
    }

    public RuleBuilder<TInstance, TProperty> MessageAsync(Func<TProperty, CancellationToken, Task<string>> func) =>
        MessageAsync((prop, _, ct) => func(prop, ct));

    public RuleBuilder<TInstance, TProperty> MessageAsync(Func<CancellationToken, Task<string>> func) =>
        MessageAsync((_, _, ct) => func(ct));

    public RuleBuilder<TInstance, TProperty> MessageAsync(Func<TProperty, TInstance, Task<string>> func) =>
        MessageAsync((prop, inst, _) => func(prop, inst));

    public RuleBuilder<TInstance, TProperty> MessageAsync(Func<TProperty, Task<string>> func) =>
        MessageAsync((prop, _, _) => func(prop));

    public RuleBuilder<TInstance, TProperty> SetValidator(ValidatorBuilder<TProperty> validator)
    {
        var rule = new ValidationRule<TInstance, TProperty>(this._propertySelector, validator);
        this.ApplyCascade(rule);
        this._currentRule = rule;
        this._rules.Add(rule);
        return this;
    }

    public RuleBuilder<TInstance, TProperty> SetValidator<TNested>(ValidatorBuilder<TNested> validator)
    {
        var rule = new ValidationRule<TInstance, TProperty>(this._propertySelector, (INestedValidator)validator);
        this.ApplyCascade(rule);
        this._currentRule = rule;
        this._rules.Add(rule);
        return this;
    }

    public RuleBuilder<TInstance, TProperty> MustAsync(Func<TProperty, CancellationToken, Task<bool>> condition)
    {
        var rule = new AsyncValidationRule<TInstance, TProperty>(this._propertySelector, condition);
        this.ApplyCascade(rule);
        this._currentRule = rule;
        this._rules.Add(rule);
        this._internalRules.Add(rule);
        return this;
    }

    public RuleBuilder<TInstance, TProperty> MustAsync(Func<TProperty, TInstance, CancellationToken, Task<bool>> condition)
    {
        var rule = new AsyncValidationRule<TInstance, TProperty>(this._propertySelector, condition);
        this.ApplyCascade(rule);
        this._currentRule = rule;
        this._rules.Add(rule);
        this._internalRules.Add(rule);
        return this;
    }

    internal RuleBuilder<TInstance, TProperty> MustAsync(Expression<Func<TProperty, CancellationToken, Task<bool>>> condition, Expression<Func<TProperty, TInstance, string>> func)
    {
        var compiled = func.Compile();
        var rule = new AsyncValidationRule<TInstance, TProperty>(this._propertySelector, condition)
        {
            ErrorMessageFunc = (property, instance) => compiled(property, instance)
        };
        this.ApplyCascade(rule);
        this._currentRule = rule;
        this._rules.Add(rule);
        this._internalRules.Add(rule);
        return this;
    }

    internal RuleBuilder<TInstance, TProperty> MustAsync(Expression<Func<TProperty, TInstance, CancellationToken, Task<bool>>> condition, Expression<Func<TProperty, TInstance, string>> func)
    {
        var compiled = func.Compile();
        var rule = new AsyncValidationRule<TInstance, TProperty>(this._propertySelector, condition)
        {
            ErrorMessageFunc = (property, instance) => compiled(property, instance)
        };
        this.ApplyCascade(rule);
        this._currentRule = rule;
        this._rules.Add(rule);
        this._internalRules.Add(rule);
        return this;
    }

    public RuleBuilder<TInstance, TProperty> SetCascadeMode(CascadeMode mode)
    {
        this._cascadeMode = mode;
        foreach (var r in this._internalRules)
            r.CascadeMode = mode;
        return this;
    }

    // When/WhenAsync apply to all rules defined on this builder so far,
    // allowing a group of Must/MustAsync rules to share a single condition.
    public RuleBuilder<TInstance, TProperty> When(Func<TInstance, bool> predicate)
    {
        if (predicate is null) throw new ArgumentNullException(nameof(predicate));
        foreach (var rule in this._internalRules)
            rule.When = predicate;
        return this;
    }

    public RuleBuilder<TInstance, TProperty> WhenAsync(Func<TInstance, CancellationToken, Task<bool>> predicateAsync)
    {
        if (predicateAsync is null) throw new ArgumentNullException(nameof(predicateAsync));
        foreach (var rule in this._internalRules)
            rule.WhenAsync = predicateAsync;
        return this;
    }

    public RuleBuilder<TInstance, TProperty> WhenAsync(Func<TInstance, Task<bool>> predicateAsync) =>
        WhenAsync((t, ct) => predicateAsync(t));
}
