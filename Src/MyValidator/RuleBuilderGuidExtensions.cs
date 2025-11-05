namespace Mert1s.MyValidator;
/// <summary>
/// Extensões para validar valores GUID dentro de RuleBuilder.
/// </summary>
public static class RuleBuilderGuidExtensions
{
    /// <summary>
    /// Garante que o GUID não seja vazio (Empty).
    /// </summary>
    public static RuleBuilder<TInstance, Guid> NotEmpty<TInstance>(this RuleBuilder<TInstance, Guid> builder) => builder.Must(x => x != Guid.Empty, (x, _) => $"{x} is empty");

    /// <summary>
    /// Garante que o GUID seja válido.
    /// </summary>
    public static RuleBuilder<TInstance, Guid> IsValidFormat<TInstance>(this RuleBuilder<TInstance, Guid> builder) => builder.Must(x => ValidateGuid(x), (x, _) => $"{x} is invalid");

    /// <summary>
    /// Método auxiliar para validar GUID.
    /// </summary>
    private static bool ValidateGuid(Guid guid) => Guid.TryParse(guid.ToString(), out _);

    /// <summary>
    /// Garante que o GUID seja um dos valores permitidos.
    /// </summary>
    /// <param name="allowedGuids">Lista de GUIDs permitidos.</param>
    public static RuleBuilder<TInstance, Guid> IsOneOf<TInstance>(this RuleBuilder<TInstance, Guid> builder, params Guid[] allowedGuids) => builder.Must(x => Array.Exists(allowedGuids, guid => guid == x), (x, _) => $"{x} is not an allowed value");

    /// <summary>
    /// Garante que o GUID não seja um dos valores proibidos.
    /// </summary>
    /// <param name="disallowedGuids">Lista de GUIDs proibidos.</param>
    public static RuleBuilder<TInstance, Guid> IsNotOneOf<TInstance>(this RuleBuilder<TInstance, Guid> builder, params Guid[] disallowedGuids) => builder.Must(x => !Array.Exists(disallowedGuids, guid => guid == x), (x, _) => $"{x} is an unallowed value");
}
