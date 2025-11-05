namespace Mert1s.MyValidator;

public static class RuleBuilderFloatExtensions
{
    public static RuleBuilder<TInstance, float> GreaterThan<TInstance>(this RuleBuilder<TInstance, float> builder, float value) => builder.Must((x) => x > value, (x, _) => $"{x} is not greater than {value}");
    public static RuleBuilder<TInstance, float> GreaterThanOrEqual<TInstance>(this RuleBuilder<TInstance, float> builder, float value) => builder.Must(x => x >= value, (x, _) => $"{x} is not greater than or equal to {value}");
    public static RuleBuilder<TInstance, float> LessThan<TInstance>(this RuleBuilder<TInstance, float> builder, float value) => builder.Must(x => x < value, (x, _) => $"{x} is not less than {value}");
    public static RuleBuilder<TInstance, float> LessThanOrEqual<TInstance>(this RuleBuilder<TInstance, float> builder, float value) => builder.Must(x => x <= value, (x, _) => $"{x} is not less than or equal to {value}");
    public static RuleBuilder<TInstance, float> Between<TInstance>(this RuleBuilder<TInstance, float> builder, float min, float max) => builder.Must(x => x >= min && x <= max, (x, _) => $"{x} is not between {min} and {max}");
    public static RuleBuilder<TInstance, float> IsPositive<TInstance>(this RuleBuilder<TInstance, float> builder) => builder.Must(x => x > 0, (x, _) => $"{x} is not positive");
    public static RuleBuilder<TInstance, float> IsNegative<TInstance>(this RuleBuilder<TInstance, float> builder) => builder.Must(x => x < 0, (x, _) => $"{x} is not negative");
}
