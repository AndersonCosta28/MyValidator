namespace UnitTests;

internal class StringModel { public string Value { get; set; } = string.Empty; }

public class RuleBuilderStringExtensionsTests
{
    [Fact]
    public void NotEmpty_Should_Fail_When_Empty()
    {
        var model = new StringModel { Value = string.Empty };
        var validator = new TestValidatorEmpty();

        var result = validator.Validate(model);

        Assert.Contains(result.SelectMany(r => r.Errors), e => e.Message.Contains("is empty"));
    }

    [Fact]
    public void NotNullOrWhiteSpace_Should_Fail_When_Whitespace()
    {
        var model = new StringModel { Value = " " };
        var validator = new TestValidatorNullOrWhitespace();

        var result = validator.Validate(model);

        Assert.Contains(result.SelectMany(r => r.Errors), e => e.Message.Contains("is null or whitespace"));
    }

    [Fact]
    public void Length_Should_Fail_When_WrongLength()
    {
        var model = new StringModel { Value = "ab" };
        var validator = new TestValidatorLength();

        var result = validator.Validate(model);

        Assert.Contains(result.SelectMany(r => r.Errors), e => e.Message.Contains("must have length 3"));
    }

    [Fact]
    public void MinMaxLength_Should_Fail_When_OutOfRange()
    {
        var modelShort = new StringModel { Value = "a" };
        var modelLong = new StringModel { Value = new string('x', 6) };
        var validatorMin = new TestValidatorMinLength();
        var validatorMax = new TestValidatorMaxLength();

        var resultMin = validatorMin.Validate(modelShort);
        var resultMax = validatorMax.Validate(modelLong);

        Assert.Contains(resultMin.SelectMany(r => r.Errors), e => e.Message.Contains("is below the min lenght"));
        Assert.Contains(resultMax.SelectMany(r => r.Errors), e => e.Message.Contains("is above the max lenght"));
    }

    [Fact]
    public void Contains_StartsWith_EndsWith_Matches_Should_Work()
    {
        var model = new StringModel { Value = "hello world" };
        var validator = new TestValidatorStringChecks();

        var result = validator.Validate(model);

        // All rules are satisfied, so no errors
        Assert.All(result, r => Assert.True(r.IsValid));
    }

    // Validators used in tests
    private class TestValidatorEmpty : ValidatorBuilder<StringModel>
    {
        public TestValidatorEmpty() => this.RuleFor(x => x.Value).NotEmpty();
    }

    private class TestValidatorNullOrWhitespace : ValidatorBuilder<StringModel>
    {
        public TestValidatorNullOrWhitespace() => this.RuleFor(x => x.Value).NotNullOrWhiteSpace();
    }

    private class TestValidatorLength : ValidatorBuilder<StringModel>
    {
        public TestValidatorLength() => this.RuleFor(x => x.Value).Length(3);
    }

    private class TestValidatorMinLength : ValidatorBuilder<StringModel>
    {
        public TestValidatorMinLength() => this.RuleFor(x => x.Value).MinLength(2);
    }

    private class TestValidatorMaxLength : ValidatorBuilder<StringModel>
    {
        public TestValidatorMaxLength() => this.RuleFor(x => x.Value).MaxLength(5);
    }

    private class TestValidatorStringChecks : ValidatorBuilder<StringModel>
    {
        public TestValidatorStringChecks() => this.RuleFor(x => x.Value)
                .Contains("world")
                .StartsWith("hello")
                .EndsWith("world")
                .Matches("^hello\\sworld$");
    }
}
