namespace UnitTests;
internal enum SampleEnum { A = 1, B = 2 }

internal class EnumModel { public SampleEnum Value { get; set; } }

public class RuleBuilderEnumExtensionsTests
{
    [Fact]
    public void Enum_Extensions_Should_Produce_Expected_Messages()
    {
        var model = new EnumModel { Value = (SampleEnum)999 };
        var validator = new EnumValidator();
        var messages = validator.Validate(model).SelectMany(r => r.Errors).Select(e => e.Message).ToList();

        Assert.Contains($"{(SampleEnum)999} is not a valid SampleEnum", messages);
        Assert.Contains($"{(SampleEnum)999} is not an allowed value", messages);
    }

    [Fact]
    public void Enum_IsOneOf_Should_Pass_When_Allowed()
    {
        var model = new EnumModel { Value = SampleEnum.A };
        var validator = new EnumValidatorAllowed();
        var results = validator.Validate(model);

        Assert.All(results, r => Assert.True(r.IsValid));
    }

    private class EnumValidator : ValidatorBuilder<EnumModel>
    {
        public EnumValidator() => this.RuleFor(x => x.Value)
            .IsDefined()
            .IsOneOf(SampleEnum.A);
    }

    private class EnumValidatorAllowed : ValidatorBuilder<EnumModel>
    {
        public EnumValidatorAllowed() => this.RuleFor(x => x.Value).IsOneOf(SampleEnum.A, SampleEnum.B);
    }
}
