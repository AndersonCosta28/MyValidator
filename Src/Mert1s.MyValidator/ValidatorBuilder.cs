namespace Mert1s.MyValidator;
public enum CascadeMode
{
    Continue = 0,
    Stop = 1
}

public abstract partial class ValidatorBuilder<T> : INestedValidator
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

    /// <summary>
    /// Adiciona regras condicionalmente. As regras criadas dentro da ação serão avaliadas
    /// apenas quando o predicado informado retornar <c>true</c> para a instância atual.
    /// </summary>
    /// <param name="predicate">Predicado que verifica se as regras devem ser executadas.</param>
    /// <param name="configure">Ação que registra regras via <see cref="RuleFor{TProperty}(Expression{Func{T, TProperty}})"/> ou <see cref="RulesFor{TItem}(Expression{Func{T, IEnumerable{TItem}}})"/>.</param>
    protected WhenHandle When(Func<T, bool> predicate, Action configure)
    {
        if (predicate is null) throw new ArgumentNullException(nameof(predicate));
        if (configure is null) throw new ArgumentNullException(nameof(configure));

        // Marca quantas regras já existem; as novas adicionadas pela ação serão envolvidas.
        var startIndex = this._rules.Count;

        configure();
        // Para cada regra adicionada pela ação, substitui por uma versão condicional.
        var wrappers = new List<IValidationRule<T>>();
        for (var i = startIndex; i < this._rules.Count; i++)
        {
            var inner = this._rules[i];
            var wrapper = new ConditionalValidationRule<T>(predicate, inner);
            this._rules[i] = wrapper;
            wrappers.Add(wrapper);
        }

        return new WhenHandle(this, wrappers);
    }

    /// <summary>
    /// Versão assíncrona do When que aceita um predicado assíncrono. As regras criadas
    /// dentro da ação serão avaliadas apenas quando o predicado assíncrono retornar true.
    /// </summary>
    protected WhenHandle WhenAsync(Func<T, CancellationToken, Task<bool>> predicateAsync, Action configure)
    {
        if (predicateAsync is null) throw new ArgumentNullException(nameof(predicateAsync));
        if (configure is null) throw new ArgumentNullException(nameof(configure));

        var startIndex = this._rules.Count;

        configure();
        var wrappers = new List<IValidationRule<T>>();
        for (var i = startIndex; i < this._rules.Count; i++)
        {
            var inner = this._rules[i];
            var wrapper = new ConditionalAsyncValidationRule<T>(predicateAsync, inner);
            this._rules[i] = wrapper;
            wrappers.Add(wrapper);
        }

        return new WhenHandle(this, wrappers);
    }

    /// <summary>
    /// Conveniência que aceita um predicate assíncrono sem CancellationToken.
    /// </summary>
    protected void WhenAsync(Func<T, Task<bool>> predicateAsync, Action configure) =>
        WhenAsync((t, ct) => predicateAsync(t), configure);

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