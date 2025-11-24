using System.Collections.Generic;
using System.Linq;
using Mert1s.MyValidator;
using Xunit;

namespace UnitTests;

public class AdditionalExtensionsTests
{
    // Models
    private class ModelWithList { public List<int> Values { get; set; } = new(); }
    private class ModelWithHashSet { public HashSet<int> Values { get; set; } = new(); }
    private class ModelWithArray { public int[] Values { get; set; } = System.Array.Empty<int>(); }
    private class ModelWithReadOnlyList { public IReadOnlyList<int> Values { get; set; } = System.Array.Empty<int>(); }
    private class ModelWithISet { public ISet<int> Values { get; set; } = new HashSet<int>(); }

    private class ModelWithNullableInt { public int? Value { get; set; } }
    private class ModelWithNonNullString { public string Name { get; set; } = string.Empty; }

    // Tests for concrete collection overloads
    [Fact]
    public void List_NotEmpty_Fails_When_Empty()
    {
        var m = new ModelWithList { Values = new List<int>() };
        var v = new TestValidatorListNotEmpty();
        var res = v.Validate(m);
        Assert.Contains(res.SelectMany(r => r.Errors), e => e.Message.Contains("Collection must contain at least one element"));
    }

    [Fact]
    public void HashSet_NotEmpty_Fails_When_Empty()
    {
        var m = new ModelWithHashSet { Values = new HashSet<int>() };
        var v = new TestValidatorHashSetNotEmpty();
        var res = v.Validate(m);
        Assert.Contains(res.SelectMany(r => r.Errors), e => e.Message.Contains("Collection must contain at least one element"));
    }

    [Fact]
    public void Array_NotEmpty_Fails_When_Empty()
    {
        var m = new ModelWithArray { Values = new int[0] };
        var v = new TestValidatorArrayNotEmpty();
        var res = v.Validate(m);
        Assert.Contains(res.SelectMany(r => r.Errors), e => e.Message.Contains("Collection must contain at least one element"));
    }

    [Fact]
    public void ReadOnlyList_NotEmpty_Fails_When_Empty()
    {
        var m = new ModelWithReadOnlyList { Values = new List<int>() };
        var v = new TestValidatorReadOnlyListNotEmpty();
        var res = v.Validate(m);
        Assert.Contains(res.SelectMany(r => r.Errors), e => e.Message.Contains("Collection must contain at least one element"));
    }

    [Fact]
    public void ISet_NotEmpty_Fails_When_Empty()
    {
        var m = new ModelWithISet { Values = new HashSet<int>() };
        var v = new TestValidatorISetNotEmpty();
        var res = v.Validate(m);
        Assert.Contains(res.SelectMany(r => r.Errors), e => e.Message.Contains("Collection must contain at least one element"));
    }

    // Tests for HasAtLeast on List and HashSet
    [Fact]
    public void List_HasAtLeast_Fails_When_NotEnough()
    {
        var m = new ModelWithList { Values = new List<int> { 1 } };
        var v = new TestValidatorListHasAtLeast();
        var res = v.Validate(m);
        Assert.Contains(res.SelectMany(r => r.Errors), e => e.Message.Contains("at least 2 elements"));
    }

    [Fact]
    public void HashSet_HasAtLeast_Fails_When_NotEnough()
    {
        var m = new ModelWithHashSet { Values = new HashSet<int> { 1 } };
        var v = new TestValidatorHashSetHasAtLeast();
        var res = v.Validate(m);
        Assert.Contains(res.SelectMany(r => r.Errors), e => e.Message.Contains("at least 2 elements"));
    }

    // Tests for NotNull overloads
    [Fact]
    public void NullableInt_NotNull_Fails_When_Null()
    {
        var m = new ModelWithNullableInt { Value = null };
        var v = new TestValidatorNullableIntNotNull();
        var res = v.Validate(m);
        Assert.Contains(res.SelectMany(r => r.Errors), e => e.Message.Contains("Int32 is null") || e.Message.Contains("is null"));
    }

    [Fact]
    public void NotNull_NotNullConstraint_Fails_When_Null()
    {
        var m = new ModelWithNonNullString { Name = null! };
        var v = new TestValidatorNotNullNotnullConstraint();
        var res = v.Validate(m);
        Assert.Contains(res.SelectMany(r => r.Errors), e => e.Message.Contains("String is null") || e.Message.Contains("is null"));
    }

    // Validators used in tests
    private class TestValidatorListNotEmpty : ValidatorBuilder<ModelWithList>
    {
        public TestValidatorListNotEmpty() => this.RuleFor(x => x.Values).NotEmpty();
    }

    private class TestValidatorHashSetNotEmpty : ValidatorBuilder<ModelWithHashSet>
    {
        public TestValidatorHashSetNotEmpty() => this.RuleFor(x => x.Values).NotEmpty();
    }

    private class TestValidatorArrayNotEmpty : ValidatorBuilder<ModelWithArray>
    {
        public TestValidatorArrayNotEmpty() => this.RuleFor(x => x.Values).NotEmpty();
    }

    private class TestValidatorReadOnlyListNotEmpty : ValidatorBuilder<ModelWithReadOnlyList>
    {
        public TestValidatorReadOnlyListNotEmpty() => this.RuleFor(x => x.Values).NotEmpty();
    }

    private class TestValidatorISetNotEmpty : ValidatorBuilder<ModelWithISet>
    {
        public TestValidatorISetNotEmpty() => this.RuleFor(x => x.Values).NotEmpty();
    }

    private class TestValidatorListHasAtLeast : ValidatorBuilder<ModelWithList>
    {
        public TestValidatorListHasAtLeast() => this.RuleFor(x => x.Values).HasAtLeast(2);
    }

    private class TestValidatorHashSetHasAtLeast : ValidatorBuilder<ModelWithHashSet>
    {
        public TestValidatorHashSetHasAtLeast() => this.RuleFor(x => x.Values).HasAtLeast(2);
    }

    private class TestValidatorNullableIntNotNull : ValidatorBuilder<ModelWithNullableInt>
    {
        public TestValidatorNullableIntNotNull() => this.RuleFor(x => x.Value).NotNull();
    }

    private class TestValidatorNotNullNotnullConstraint : ValidatorBuilder<ModelWithNonNullString>
    {
        public TestValidatorNotNullNotnullConstraint() => this.RuleFor(x => x.Name).NotNull_NotNullConstraint();
    }
}
