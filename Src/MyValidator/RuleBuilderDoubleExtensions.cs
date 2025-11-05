namespace Mert1s.MyValidator;

/// <summary>
/// Extensões para validar valores double dentro de RuleBuilder.
/// </summary>
public static class RuleBuilderDoubleExtensions
{
    /// <summary>
    /// Garante que o valor seja maior que um número especificado.
    /// </summary>
    /// <param name="value">Valor mínimo esperado.</param>
    public static RuleBuilder<TInstance, double> GreaterThan<TInstance>(this RuleBuilder<TInstance, double> builder, double value) => builder.Must((x) => x > value, (x, _) => $"{x} is not greater than {value}");

    /// <summary>
    /// Garante que o valor seja maior ou igual a um número especificado.
    /// </summary>
    /// <param name="value">Valor mínimo permitido.</param>
    public static RuleBuilder<TInstance, double> GreaterThanOrEqual<TInstance>(this RuleBuilder<TInstance, double> builder, double value) => builder.Must(x => x >= value, (x, _) => $"{x} is not greater than or equal to {value}");

    /// <summary>
    /// Garante que o valor seja menor que um número especificado.
    /// </summary>
    /// <param name="value">Valor máximo esperado.</param>
    public static RuleBuilder<TInstance, double> LessThan<TInstance>(this RuleBuilder<TInstance, double> builder, double value) => builder.Must(x => x < value, (x, _) => $"{x} is not less than {value}");

    /// <summary>
    /// Garante que o valor seja menor ou igual a um número especificado.
    /// </summary>
    /// <param name="value">Valor máximo permitido.</param>
    public static RuleBuilder<TInstance, double> LessThanOrEqual<TInstance>(this RuleBuilder<TInstance, double> builder, double value) => builder.Must(x => x <= value, (x, _) => $"{x} is not less than or equal to {value}");

    /// <summary>
    /// Garante que o valor esteja dentro de um doubleervalo fechado.
    /// </summary>
    /// <param name="min">Valor mínimo permitido.</param>
    /// <param name="max">Valor máximo permitido.</param>
    public static RuleBuilder<TInstance, double> Between<TInstance>(this RuleBuilder<TInstance, double> builder, double min, double max) => builder.Must(x => x >= min && x <= max, (x, _) => $"{x} is not between {min} and {max}");

    /// <summary>
    /// Garante que o valor seja positivo.
    /// </summary>
    public static RuleBuilder<TInstance, double> IsPositive<TInstance>(this RuleBuilder<TInstance, double> builder) => builder.Must(x => x > 0, (x, _) => $"{x} is not positive");

    /// <summary>
    /// Garante que o valor seja negativo.
    /// </summary>
    public static RuleBuilder<TInstance, double> IsNegative<TInstance>(this RuleBuilder<TInstance, double> builder) => builder.Must(x => x < 0, (x, _) => $"{x} is not negative");
}
