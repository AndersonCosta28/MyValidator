namespace MyValidator;

public class CollectionRuleBuilder<TInstance, TProperty>
{
    private readonly Expression<Func<TInstance, IEnumerable<TProperty>>> _collectionSelector;
    private readonly List<IValidationRule<TInstance>> _rules;
    private ValidationRule<TInstance, IEnumerable<TProperty>> _currentRule;

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
        return this;
    }

    public CollectionRuleBuilder<TInstance, TProperty> Must(Expression<Func<IEnumerable<TProperty>, TInstance, bool>> condition)
    {
        this._currentRule = new(this._collectionSelector, condition);
        this._rules.Add((IValidationRule<TInstance>)this._currentRule);
        return this;
    }

    public CollectionRuleBuilder<TInstance, TProperty> Message(string message)
    {
        if (this._currentRule != null)
            this._currentRule.ErrorMessageFunc = (_, _) => message;

        return this;
    }

    public CollectionRuleBuilder<TInstance, TProperty> Message(Expression<Func<IEnumerable<TProperty>, TInstance, string>> func)
    {
        if (this._currentRule != null)
            this._currentRule.ErrorMessageFunc = (property, instance) => func.Compile().Invoke(property, instance);

        return this;
    }

    public CollectionRuleBuilder<TInstance, TProperty> Message(Expression<Func<IEnumerable<TProperty>, string>> func)
    {
        if (this._currentRule != null)
            this._currentRule.ErrorMessageFunc = (property, instance) => func.Compile().Invoke(property);

        return this;
    }

    public CollectionRuleBuilder<TInstance, TProperty> Message(Expression<Func<string>> func)
    {
        if (this._currentRule != null)
            this._currentRule.ErrorMessageFunc = (property, instance) => func.Compile().Invoke();

        return this;
    }
}
