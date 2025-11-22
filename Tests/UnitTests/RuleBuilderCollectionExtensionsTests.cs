using System.Collections.Generic;
using System.Linq;
using Mert1s.MyValidator;
using Xunit;

namespace UnitTests;

internal class CollectionModel { public ICollection<int> Values { get; set; } = []; }

public class RuleBuilderCollectionExtensionsTests
{
    [Fact]
    public void NotEmpty_Should_Fail_When_Empty()
    {
        var model = new CollectionModel { Values = [] };
        var validator = new TestValidatorNotEmpty();

        var result = validator.Validate(model);

        Assert.Contains(result.SelectMany(r => r.Errors), e => e.Message.Contains("Collection must contain at least one element"));
    }

    [Fact]
    public void NotEmpty_Should_Pass_When_HasItems()
    {
        var model = new CollectionModel { Values = new[] { 1 } };
        var validator = new TestValidatorNotEmpty();

        var result = validator.Validate(model);

        Assert.All(result, r => Assert.True(r.IsValid));
    }

    [Fact]
    public void HasAtLeast_Should_Fail_When_NotEnough()
    {
        var model = new CollectionModel { Values = new[] { 1, 2 } };
        var validator = new TestValidatorHasAtLeast();

        var result = validator.Validate(model);

        Assert.Contains(result.SelectMany(r => r.Errors), e => e.Message.Contains("at least 3 elements"));
    }

    [Fact]
    public void HasAtLeast_Should_Pass_When_Enough()
    {
        var model = new CollectionModel { Values = new[] { 1, 2, 3 } };
        var validator = new TestValidatorHasAtLeast();

        var result = validator.Validate(model);

        Assert.All(result, r => Assert.True(r.IsValid));
    }

    [Fact]
    public void HasCountBetween_Should_Fail_When_OutOfRange()
    {
        var modelLow = new CollectionModel { Values = new[] { 1 } };
        var modelHigh = new CollectionModel { Values = new[] { 1, 2, 3, 4, 5 } };
        var validator = new TestValidatorHasCountBetween();

        var resultLow = validator.Validate(modelLow);
        var resultHigh = validator.Validate(modelHigh);

        Assert.Contains(resultLow.SelectMany(r => r.Errors), e => e.Message.Contains("between 2 and 4"));
        Assert.Contains(resultHigh.SelectMany(r => r.Errors), e => e.Message.Contains("between 2 and 4"));
    }

    [Fact]
    public void HasCountBetween_Should_Pass_When_InRange()
    {
        var model = new CollectionModel { Values = new[] { 1, 2, 3 } };
        var validator = new TestValidatorHasCountBetween();

        var result = validator.Validate(model);

        Assert.All(result, r => Assert.True(r.IsValid));
    }

    // Validators used in tests
    private class TestValidatorNotEmpty : ValidatorBuilder<CollectionModel>
    {
        public TestValidatorNotEmpty() => this.RuleFor(x => x.Values).NotEmpty();
    }

    private class TestValidatorHasAtLeast : ValidatorBuilder<CollectionModel>
    {
        public TestValidatorHasAtLeast() => this.RuleFor(x => x.Values).HasAtLeast(3);
    }

    private class TestValidatorHasCountBetween : ValidatorBuilder<CollectionModel>
    {
        public TestValidatorHasCountBetween() => this.RuleFor(x => x.Values).HasCountBetween(2, 4);
    }
}
