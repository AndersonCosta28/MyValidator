namespace Mert1s.MyValidator;

public class CollectionRuleBuilder<TInstance, TProperty>
{
    private readonly Expression<Func<TInstance, IEnumerable<TProperty>>> _collectionSelector;
    private readonly List<IValidationRule<TInstance>> _rules;
    private ValidationRule<TInstance, IEnumerable<TProperty>> _currentRule;
    private readonly List<IValidationRule<TInstance>> _internalRules = [];


    internal CollectionRuleBuilder(Expression<Func<TInstance, IEnumerable<TProperty>>> collectionSelector, List<IValidationRule<TInstance>> rules)
    {
        this._collectionSelector = collectionSelector;
        this._rules = rules;
    }

    public CollectionRuleBuilder<TInstance, TProperty> SetValidator(ValidatorBuilder<TProperty> validator)
    {
        this._currentRule = new(this._collectionSelector, validator);

        this._rules.Add(this._currentRule);
        return this;
    }

    public CollectionRuleBuilder<TInstance, TProperty> Must(Expression<Func<IEnumerable<TProperty>, bool>> condition)
    {
        this._currentRule = new(this._collectionSelector, condition);
        this._rules.Add(this._currentRule);
        this._internalRules.Add(this._currentRule);
        return this;
    }

    public CollectionRuleBuilder<TInstance, TProperty> Must(Expression<Func<IEnumerable<TProperty>, TInstance, bool>> condition)
    {
        this._currentRule = new(this._collectionSelector, condition);
        this._rules.Add(this._currentRule);
        this._internalRules.Add(this._currentRule);
        return this;
    }

    public CollectionRuleBuilder<TInstance, TProperty> Message(string message)
    {
        foreach (var rule in this._internalRules)
            if (rule is ValidationRule<TInstance, IEnumerable<TProperty>> { ErrorMessageFunc: null } validationRule)
            {
                if (validationRule.ErrorMessageFunc != null)
                    break;
                validationRule.ErrorMessageFunc = (_, _) => message;
            }

        return this;
    }

    public CollectionRuleBuilder<TInstance, TProperty> Message(Expression<Func<IEnumerable<TProperty>, TInstance, string>> func)
    {
        foreach (var rule in this._internalRules.AsEnumerable().Reverse())
            if (rule is ValidationRule<TInstance, IEnumerable<TProperty>> { ErrorMessageFunc: null } validationRule)
            {
                if (validationRule.ErrorMessageFunc != null)
                    break;
                validationRule.ErrorMessageFunc = (property, instance) => func.Compile().Invoke(property, instance);
            }

        return this;
    }

    public CollectionRuleBuilder<TInstance, TProperty> Message(Expression<Func<IEnumerable<TProperty>, string>> func)
    {
        foreach (var rule in this._internalRules)
            if (rule is ValidationRule<TInstance, IEnumerable<TProperty>> validationRule)
            {
                if (validationRule.ErrorMessageFunc != null)
                    break;
                validationRule.ErrorMessageFunc = (property, instance) => func.Compile().Invoke(property);
            }

        return this;
    }

    public CollectionRuleBuilder<TInstance, TProperty> Message(Expression<Func<string>> func)
    {
        foreach (var rule in this._internalRules)
            if (rule is ValidationRule<TInstance, IEnumerable<TProperty>> validationRule)
            {
                if (validationRule.ErrorMessageFunc != null)
                    break;
                validationRule.ErrorMessageFunc = (property, instance) => func.Compile().Invoke();
            }

        return this;
    }
}
