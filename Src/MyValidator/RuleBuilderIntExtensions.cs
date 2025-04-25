﻿namespace MyValidator;

/// <summary>
/// Extensões para validar valores inteiros dentro de RuleBuilder.
/// </summary>
public static class RuleBuilderIntExtensions
{
    /// <summary>
    /// Garante que o valor seja maior que um número especificado.
    /// </summary>
    /// <param name="value">Valor mínimo esperado.</param>
    public static RuleBuilder<TInstance, int> GreaterThan<TInstance>(this RuleBuilder<TInstance, int> builder, int value) => builder.Must((x) => x > value, (x, _) => $"{x} is not great than {value}");

    /// <summary>
    /// Garante que o valor seja maior ou igual a um número especificado.
    /// </summary>
    /// <param name="value">Valor mínimo permitido.</param>
    public static RuleBuilder<TInstance, int> GreaterThanOrEqual<TInstance>(this RuleBuilder<TInstance, int> builder, int value) => builder.Must(x => x >= value, (x, _) => $"{x} is not great than or equal {value}");

    /// <summary>
    /// Garante que o valor seja menor que um número especificado.
    /// </summary>
    /// <param name="value">Valor máximo esperado.</param>
    public static RuleBuilder<TInstance, int> LessThan<TInstance>(this RuleBuilder<TInstance, int> builder, int value) => builder.Must(x => x < value, (x, _) => $"{x} is not less than {value}");

    /// <summary>
    /// Garante que o valor seja menor ou igual a um número especificado.
    /// </summary>
    /// <param name="value">Valor máximo permitido.</param>
    public static RuleBuilder<TInstance, int> LessThanOrEqual<TInstance>(this RuleBuilder<TInstance, int> builder, int value) => builder.Must(x => x <= value, (x, _) => $"{x} is not less than or equal {value}");

    /// <summary>
    /// Garante que o valor esteja dentro de um intervalo fechado.
    /// </summary>
    /// <param name="min">Valor mínimo permitido.</param>
    /// <param name="max">Valor máximo permitido.</param>
    public static RuleBuilder<TInstance, int> Between<TInstance>(this RuleBuilder<TInstance, int> builder, int min, int max) => builder.Must(x => x >= min && x <= max, (x, _) => $"{x} is not between {min} and {max}");

    /// <summary>
    /// Garante que o valor seja positivo.
    /// </summary>
    public static RuleBuilder<TInstance, int> IsPositive<TInstance>(this RuleBuilder<TInstance, int> builder) => builder.Must(x => x > 0, (x, _) => $"{x} is not positive");

    /// <summary>
    /// Garante que o valor seja negativo.
    /// </summary>
    public static RuleBuilder<TInstance, int> IsNegative<TInstance>(this RuleBuilder<TInstance, int> builder) => builder.Must(x => x < 0, (x, _) => $"{x} is not negative");

    /// <summary>
    /// Garante que o valor seja par.
    /// </summary>
    public static RuleBuilder<TInstance, int> IsEven<TInstance>(this RuleBuilder<TInstance, int> builder) => builder.Must(x => x % 2 == 0, (x, _) => $"{x} is not even");

    /// <summary>
    /// Garante que o valor seja ímpar.
    /// </summary>
    public static RuleBuilder<TInstance, int> IsOdd<TInstance>(this RuleBuilder<TInstance, int> builder) => builder.Must(x => x % 2 != 0, (x, _) => $"{x} is not odd");
}
