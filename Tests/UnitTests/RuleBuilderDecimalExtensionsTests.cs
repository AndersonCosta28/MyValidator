namespace UnitTests;
internal class DecimalModel { public decimal Value { get; set; } }

public class RuleBuilderDecimalExtensionsTests
{
    [Fact]
    public void Decimal_Extensions_Should_Produce_Expected_Messages()
    {
        var model = new DecimalModel { Value = 5m };
        var validator = new DecimalValidator();
        var messages = validator.Validate(model).SelectMany(r => r.Errors).Select(e => e.Message).ToList();

        Assert.Contains("5 is not greater than 10", messages);
        Assert.Contains("5 is not greater than or equal to 10", messages);
        Assert.Contains("5 is not less than 3", messages);
        Assert.Contains("5 is not less than or equal to 3", messages);
        Assert.Contains("5 is not between 1 and 4", messages);
        Assert.Contains("5 is not negative", messages);
    }

    [Fact]
    public void Decimal_Extensions_Should_Pass_When_Valid()
    {
        var model = new DecimalModel { Value = 2m };
        var validator = new DecimalValidatorValid();
        var results = validator.Validate(model);

        Assert.All(results, r => Assert.True(r.IsValid));
    }

    private class DecimalValidator : ValidatorBuilder<DecimalModel>
    {
        public DecimalValidator() => this.RuleFor(x => x.Value)
        .GreaterThan(10)
        .GreaterThanOrEqual(10)
        .LessThan(3)
        .LessThanOrEqual(3)
        .Between(1, 4)
        .IsPositive()
        .IsNegative();
    }

    private class DecimalValidatorValid : ValidatorBuilder<DecimalModel>
    {
        public DecimalValidatorValid() => this.RuleFor(x => x.Value)
        .GreaterThan(1)
        .LessThan(10)
        .Between(1, 5)
        .IsPositive();
    }
}
