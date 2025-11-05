namespace UnitTests;
internal class IntModel { public int Value { get; set; } }

public class RuleBuilderIntExtensionsTests
{
    [Fact]
    public void GreaterThan_GreaterThanOrEqual_LessThan_LessThanOrEqual_Between_IsPositive_IsNegative_IsEven_Tests()
    {
        var model = new IntModel { Value = 5 };
        var validator = new TestValidatorIntChecks();
        var result = validator.Validate(model);

        var messages = result.SelectMany(r => r.Errors).Select(e => e.Message).ToList();

        // Exact message assertions (match spacing from extension messages)
        Assert.Contains("5 is not greater than 10", messages);
        Assert.Contains("5 is not greater than or equal to 10", messages);
        Assert.Contains("5 is not less than 3", messages);
        Assert.Contains("5 is not less than or equal to 3", messages);
        Assert.Contains("5 is not between 1 and 4", messages);
        Assert.Contains("5 is not negative", messages);
        Assert.Contains("5 is not even", messages);
    }

    private class TestValidatorIntChecks : ValidatorBuilder<IntModel>
    {
        public TestValidatorIntChecks() => this.RuleFor(x => x.Value)
        .GreaterThan(10)
        .GreaterThanOrEqual(10)
        .LessThan(3)
        .LessThanOrEqual(3)
        .Between(1, 4)
        .IsPositive()
        .IsNegative()
        .IsEven()
        .IsOdd();
    }
}
