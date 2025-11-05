namespace Mert1s.MyValidator;
public abstract class ValidatorBuilder<T> : INestedValidator
{
    private readonly List<IValidationRule<T>> _rules = [];

    protected RuleBuilder<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> propertySelector) =>
        new(propertySelector, this._rules);

    protected CollectionRuleBuilder<T, TItem> RulesFor<TItem>(Expression<Func<T, IEnumerable<TItem>>> selector) =>
        new(selector, this._rules);

    List<ValidationResult> INestedValidator.Validate(object instance) => this.Validate((T)instance);

    public List<ValidationResult> Validate(T instance)
    {
        List<ValidationResult> results = [];

        foreach (var rule in this._rules)
        {
            var result = rule.Validate(instance);
            results.Add(result);
        }

        return results;
    }
}