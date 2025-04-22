namespace MyValidator;

public class RuleBuilder<TInstance, TProperty>
{
    private readonly Expression<Func<TInstance, TProperty>> _propertySelector;
    private readonly List<IValidationRule<TInstance>> _rules;
    private readonly List<IValidationRule<TInstance>> _internalRules = [];
    private ValidationRule<TInstance, TProperty> _currentRule;

    internal RuleBuilder(Expression<Func<TInstance, TProperty>> propertySelector, List<IValidationRule<TInstance>> rules)
    {
        this._propertySelector = propertySelector;
        this._rules = rules;
    }

    public RuleBuilder<TInstance, TProperty> Must(Expression<Func<TProperty, bool>> condition)
    {
        this._currentRule = new(this._propertySelector, condition);
        this._rules.Add(this._currentRule);
        this._internalRules.Add(this._currentRule);
        return this;
    }

    public RuleBuilder<TInstance, TProperty> Must(Expression<Func<TProperty, TInstance, bool>> condition)
    {
        this._currentRule = new(this._propertySelector, condition);
        this._rules.Add(this._currentRule);
        this._internalRules.Add(this._currentRule);
        return this;
    }

    public RuleBuilder<TInstance, TProperty> Message(string message)
    {
        foreach (var rule in this._internalRules)
            if (rule is ValidationRule<TInstance, TProperty> { ErrorMessageFunc: null } validationRule)
            {
                if (validationRule.ErrorMessageFunc != null)
                    break;
                validationRule.ErrorMessageFunc = (_, _) => message;
            }

        return this;
    }

    public RuleBuilder<TInstance, TProperty> Message(Expression<Func<TProperty, TInstance, string>> func)
    {
        foreach (var rule in this._internalRules.AsEnumerable().Reverse())
            if (rule is ValidationRule<TInstance, TProperty> { ErrorMessageFunc: null } validationRule)
            {
                if (validationRule.ErrorMessageFunc != null)
                    break;
                validationRule.ErrorMessageFunc = (property, instance) => func.Compile().Invoke(property, instance);
            }

        return this;
    }

    public RuleBuilder<TInstance, TProperty> Message(Expression<Func<TProperty, string>> func)
    {
        foreach (var rule in this._internalRules)
            if (rule is ValidationRule<TInstance, TProperty> validationRule)
            {
                if (validationRule.ErrorMessageFunc != null)
                    break;
                validationRule.ErrorMessageFunc = (property, instance) => func.Compile().Invoke(property);
            }

        return this;
    }

    public RuleBuilder<TInstance, TProperty> Message(Expression<Func<string>> func)
    {
        foreach (var rule in this._internalRules)
            if (rule is ValidationRule<TInstance, TProperty> validationRule)
            {
                if (validationRule.ErrorMessageFunc != null)
                    break;
                validationRule.ErrorMessageFunc = (property, instance) => func.Compile().Invoke();
            }

        return this;
    }

    public RuleBuilder<TInstance, TProperty> SetValidator(ValidatorBuilder<TProperty> validator)
    {
        this._currentRule = new(this._propertySelector, validator);

        this._rules.Add(this._currentRule);
        return this;
    }
}
