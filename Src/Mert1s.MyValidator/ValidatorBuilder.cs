namespace Mert1s.MyValidator;
public enum CascadeMode
{
    Continue = 0,
    Stop = 1
}

public abstract class ValidatorBuilder<T> : INestedValidator
{
    private readonly List<IValidationRule<T>> _rules = [];
    /// <summary>
    /// Controla o comportamento de cascata das regras: quando configurado como <see cref="CascadeMode.Stop"/>,
    /// regras adicionais para a mesma propriedade serão ignoradas após a primeira falha.
    /// </summary>
    public CascadeMode CascadeMode { get; set; } = CascadeMode.Continue;

    protected RuleBuilder<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> propertySelector) =>
    new(propertySelector, this._rules);

    protected CollectionRuleBuilder<T, TItem> RulesFor<TItem>(Expression<Func<T, IEnumerable<TItem>>> selector) =>
    new(selector, this._rules);

    List<ValidationResult> INestedValidator.Validate(object instance) => this.Validate((T)instance);

    public List<ValidationResult> Validate(T instance)
    {
        List<ValidationResult> results = [];
        var failedPaths = new HashSet<string>();

        foreach (var rule in this._rules)
        {
            var path = rule.PathName;
            var effective = rule.CascadeMode ?? this.CascadeMode;
            if (effective == CascadeMode.Stop && failedPaths.Contains(path))
            {
                // Skip this rule because a previous rule for the same property failed.
                continue;
            }

            var result = rule.Validate(instance);
            results.Add(result);

            if (result.Errors.Count > 0)
                failedPaths.Add(path);
        }

        return results;
    }

    Task<List<ValidationResult>> INestedValidator.ValidateAsync(object instance, CancellationToken cancellation) =>
    this.ValidateAsync((T)instance, cancellation);

    public async Task<List<ValidationResult>> ValidateAsync(T instance, CancellationToken cancellation = default)
    {
        List<ValidationResult> results = [];
        var failedPaths = new HashSet<string>();

        foreach (var rule in this._rules)
        {
            var path = rule.PathName;
            var effective = rule.CascadeMode ?? this.CascadeMode;
            if (effective == CascadeMode.Stop && failedPaths.Contains(path))
            {
                continue;
            }

            var result = await rule.ValidateAsync(instance, cancellation).ConfigureAwait(false);
            results.Add(result);

            if (result.Errors.Count > 0)
                failedPaths.Add(path);
        }

        return results;
    }
}