using System.Threading;

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

 Task<List<ValidationResult>> INestedValidator.ValidateAsync(object instance, CancellationToken cancellation) =>
 this.ValidateAsync((T)instance, cancellation);

 public async Task<List<ValidationResult>> ValidateAsync(T instance, CancellationToken cancellation = default)
 {
 List<ValidationResult> results = [];

 foreach (var rule in this._rules)
 {
 var result = await rule.ValidateAsync(instance, cancellation).ConfigureAwait(false);
 results.Add(result);
 }

 return results;
 }
}