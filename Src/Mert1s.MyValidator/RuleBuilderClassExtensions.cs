namespace Mert1s.MyValidator;

/// <summary>
/// Extensões para validar referências/objetos (null checks etc.).
/// </summary>
public static class RuleBuilderClassExtensions
{
    /// <summary>
    /// Garante que a propriedade de referência não seja nula.
    /// </summary>
    public static RuleBuilder<TInstance, TProperty> NotNull<TInstance, TProperty>(this RuleBuilder<TInstance, TProperty> builder)
        where TProperty : class
        => builder.Must(x => x != null, (x, _) => $"{typeof(TProperty).Name} is null");

    /// <summary>
    /// Garante que a propriedade Nullable&lt;T&gt; tenha valor.
    /// </summary>
    public static RuleBuilder<TInstance, TProperty?> NotNull<TInstance, TProperty>(this RuleBuilder<TInstance, TProperty?> builder)
        where TProperty : struct
        => builder.Must(x => x.HasValue, (x, _) => $"{typeof(TProperty).Name} is null");

    /// <summary>
    /// Versão que aceita qualquer tipo não-nulo (C# 8+). Útil quando se deseja aplicar a regra
    /// para tipos que são known-not-null em contexto de nullability.
    /// </summary>
    public static RuleBuilder<TInstance, TProperty> NotNull_NotNullConstraint<TInstance, TProperty>(this RuleBuilder<TInstance, TProperty> builder)
        where TProperty : notnull
        => builder.Must(x => x != null, (x, _) => $"{typeof(TProperty).Name} is null");
}
