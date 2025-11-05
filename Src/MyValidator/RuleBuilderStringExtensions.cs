namespace Mert1s.MyValidator;
/// <summary>
/// Extensões para validar strings dentro de RuleBuilder.
/// </summary>
public static class RuleBuilderStringExtensions
{
    /// <summary>
    /// Garante que a string não seja nula nem vazia.
    /// </summary>
    public static RuleBuilder<TInstance, string> NotEmpty<TInstance>(this RuleBuilder<TInstance, string> builder) => builder.Must(x => !string.IsNullOrEmpty(x), (x, _) => $"{x} is empty");

    /// <summary>
    /// Garante que a string não seja nula, vazia ou composta apenas por espaços em branco.
    /// </summary>
    public static RuleBuilder<TInstance, string> NotNullOrWhiteSpace<TInstance>(this RuleBuilder<TInstance, string> builder) => builder.Must(x => !string.IsNullOrWhiteSpace(x), (x, _) => $"{x} is null or whitespace");

    /// <summary>
    /// Garante que a string tenha exatamente o tamanho especificado.
    /// </summary>
    /// <param name="length">Comprimento exato esperado.</param>
    public static RuleBuilder<TInstance, string> Length<TInstance>(this RuleBuilder<TInstance, string> builder, int length)
        => builder.Must(x => x.Length == length, (x, _) => $"{x} must have length {length}");

    /// <summary>
    /// Garante que a string tenha um tamanho mínimo especificado.
    /// </summary>
    /// <param name="length">Comprimento mínimo esperado.</param>
    public static RuleBuilder<TInstance, string> MinLength<TInstance>(this RuleBuilder<TInstance, string> builder, int length) => builder.Must(x => x.Length >= length, (x, _) => $"{x} is below the min lenght  {length}");

    /// <summary>
    /// Garante que a string tenha um tamanho máximo especificado.
    /// </summary>
    /// <param name="length">Comprimento máximo permitido.</param>
    public static RuleBuilder<TInstance, string> MaxLength<TInstance>(this RuleBuilder<TInstance, string> builder, int length) => builder.Must(x => x.Length <= length, (x, _) => $"{x} is above the max lenght {length}");

    /// <summary>
    /// Garante que a string tenha um tamanho dentro de um intervalo especificado.
    /// </summary>
    /// <param name="min">Comprimento mínimo permitido.</param>
    /// <param name="max">Comprimento máximo permitido.</param>
    public static RuleBuilder<TInstance, string> LengthBetween<TInstance>(this RuleBuilder<TInstance, string> builder, int min, int max)
        => builder.Must(x => x.Length >= min && x.Length <= max, (x, _) => $"{x} must be between {min} and {max} characters");

    /// <summary>
    /// Garante que a string contenha um determinado valor.
    /// </summary>
    /// <param name="value">Texto esperado dentro da string.</param>
    public static RuleBuilder<TInstance, string> Contains<TInstance>(this RuleBuilder<TInstance, string> builder, string value)
        => builder.Must(x => x.Contains(value), (x, _) => $"'{x}' must contain '{value}'");

    /// <summary>
    /// Garante que a string comece com um determinado valor.
    /// </summary>
    /// <param name="value">Prefixo esperado.</param>
    public static RuleBuilder<TInstance, string> StartsWith<TInstance>(this RuleBuilder<TInstance, string> builder, string value)
        => builder.Must(x => x.StartsWith(value), (x, _) => $"'{x}' must start with '{value}'");

    /// <summary>
    /// Garante que a string termine com um determinado valor.
    /// </summary>
    /// <param name="value">Sufixo esperado.</param>
    public static RuleBuilder<TInstance, string> EndsWith<TInstance>(this RuleBuilder<TInstance, string> builder, string value)
        => builder.Must(x => x.EndsWith(value), (x, _) => $"'{x}' must end with '{value}'");

    /// <summary>
    /// Garante que a string corresponda a um padrão de expressão regular.
    /// </summary>
    /// <param name="pattern">Expressão regular a ser validada.</param>
    public static RuleBuilder<TInstance, string> Matches<TInstance>(this RuleBuilder<TInstance, string> builder, string pattern)
        => builder.Must(x => System.Text.RegularExpressions.Regex.IsMatch(x, pattern), (x, _) => $"'{x}' does not match the expected pattern: '{pattern}'");
}
