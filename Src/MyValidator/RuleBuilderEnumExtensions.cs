namespace Mert1s.MyValidator;
/// <summary>
/// Extensões para validar valores de enum dentro de RuleBuilder.
/// </summary>
public static class RuleBuilderEnumExtensions
{
    /// <summary>
    /// Garante que o valor do enum está definido dentro da enumeração correspondente.
    /// </summary>
    public static RuleBuilder<TInstance, TProperty> IsDefined<TInstance, TProperty>(this RuleBuilder<TInstance, TProperty> builder)
        where TProperty : struct, Enum => builder.Must(x => Enum.IsDefined(typeof(TProperty), x));

    /// <summary>
    /// Garante que o valor do enum seja um dos valores permitidos.
    /// </summary>
    /// <param name="allowedValues">Lista de valores permitidos.</param>
    public static RuleBuilder<TInstance, TProperty> IsOneOf<TInstance, TProperty>(this RuleBuilder<TInstance, TProperty> builder, params TProperty[] allowedValues)
        where TProperty : struct, Enum => builder.Must(x => Array.Exists(allowedValues, v => EqualityComparer<TProperty>.Default.Equals(v, x)));

    /// <summary>
    /// Garante que o valor do enum não seja um dos valores proibidos.
    /// </summary>
    /// <param name="disallowedValues">Lista de valores proibidos.</param>
    public static RuleBuilder<TInstance, TProperty> IsNotOneOf<TInstance, TProperty>(this RuleBuilder<TInstance, TProperty> builder, params TProperty[] disallowedValues)
        where TProperty : struct, Enum => builder.Must(x => !Array.Exists(disallowedValues, v => EqualityComparer<TProperty>.Default.Equals(v, x)));
}
