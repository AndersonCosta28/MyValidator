namespace MyValidator;

/// <summary>
/// Extensões para validar valores float dentro de RuleBuilder.
/// </summary>
public static class RuleBuilderFloatExtensions
{
    /// <summary>
    /// Garante que o valor seja maior que um número especificado.
    /// </summary>
    /// <param name="value">Valor mínimo esperado.</param>
    public static RuleBuilder<TInstance, float> GreaterThan<TInstance>(this RuleBuilder<TInstance, float> builder, float value) => builder.Must((x) => x > value, (x, _) => $"{x} is not great than {value}");

    /// <summary>
    /// Garante que o valor seja maior ou igual a um número especificado.
    /// </summary>
    /// <param name="value">Valor mínimo permitido.</param>
    public static RuleBuilder<TInstance, float> GreaterThanOrEqual<TInstance>(this RuleBuilder<TInstance, float> builder, float value) => builder.Must(x => x >= value, (x, _) => $"{x} is not great than or equal {value}");

    /// <summary>
    /// Garante que o valor seja menor que um número especificado.
    /// </summary>
    /// <param name="value">Valor máximo esperado.</param>
    public static RuleBuilder<TInstance, float> LessThan<TInstance>(this RuleBuilder<TInstance, float> builder, float value) => builder.Must(x => x < value, (x, _) => $"{x} is not less than {value}");

    /// <summary>
    /// Garante que o valor seja menor ou igual a um número especificado.
    /// </summary>
    /// <param name="value">Valor máximo permitido.</param>
    public static RuleBuilder<TInstance, float> LessThanOrEqual<TInstance>(this RuleBuilder<TInstance, float> builder, float value) => builder.Must(x => x <= value, (x, _) => $"{x} is not less than or equal {value}");

    /// <summary>
    /// Garante que o valor esteja dentro de um floatervalo fechado.
    /// </summary>
    /// <param name="min">Valor mínimo permitido.</param>
    /// <param name="max">Valor máximo permitido.</param>
    public static RuleBuilder<TInstance, float> Between<TInstance>(this RuleBuilder<TInstance, float> builder, float min, float max) => builder.Must(x => x >= min && x <= max, (x, _) => $"{x} is not between {min} and {max}");

    /// <summary>
    /// Garante que o valor seja positivo.
    /// </summary>
    public static RuleBuilder<TInstance, float> IsPositive<TInstance>(this RuleBuilder<TInstance, float> builder) => builder.Must(x => x > 0, (x, _) => $"{x} is not positive");

    /// <summary>
    /// Garante que o valor seja negativo.
    /// </summary>
    public static RuleBuilder<TInstance, float> IsNegative<TInstance>(this RuleBuilder<TInstance, float> builder) => builder.Must(x => x < 0, (x, _) => $"{x} is not negative");
}
