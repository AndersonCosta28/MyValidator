using System.Threading.Tasks;
using Mert1s.MyValidator;

namespace UnitTests;

public class WhenTests
{
    private class Model { public string Value { get; set; } = string.Empty; }

    [Fact]
    public void When_True_ExecutesRules()
    {
        var validator = new WhenTrueValidator();
        var results = validator.Validate(new Model { Value = "x" });
        var messages = results.SelectMany(r => r.Errors).Select(e => e.Message).ToList();

        Assert.Contains(messages, m => m.Contains("A"));
        Assert.Contains(messages, m => m.Contains("B"));
    }

    private class WhenTrueValidator : ValidatorBuilder<Model>
    {
        public WhenTrueValidator()
        {
            this.When(x => true, () =>
            {
                this.RuleFor(r => r.Value)
                    .Must(v => false).Message("A")
                    .Must(v => false).Message("B");
            });
        }
    }

    [Fact]
    public void When_False_SkipsRules()
    {
        var validator = new WhenFalseValidator();
        var results = validator.Validate(new Model { Value = "x" });
        var errors = results.SelectMany(r => r.Errors).ToList();

        Assert.Empty(errors);
    }

    private class WhenFalseValidator : ValidatorBuilder<Model>
    {
        public WhenFalseValidator()
        {
            this.When(x => false, () =>
            {
                this.RuleFor(r => r.Value)
                    .Must(v => false).Message("A")
                    .Must(v => false).Message("B");
            });
        }
    }

    [Fact]
    public void When_Respects_GlobalCascadeStop()
    {
        var validator = new WhenGlobalStopValidator();
        var results = validator.Validate(new Model { Value = "x" });
        var messages = results.SelectMany(r => r.Errors).Select(e => e.Message).ToList();

        Assert.Single(messages);
        Assert.Contains(messages, m => m.Contains("A"));
    }

    private class WhenGlobalStopValidator : ValidatorBuilder<Model>
    {
        public WhenGlobalStopValidator()
        {
            this.CascadeMode = CascadeMode.Stop;

            this.When(x => true, () =>
            {
                this.RuleFor(r => r.Value)
                    .Must(v => false).Message("A")
                    .Must(v => false).Message("B");
            });
        }
    }

    [Fact]
    public void When_Allows_PerProperty_Override()
    {
        var validator = new WhenPerPropertyOverrideValidator();
        var results = validator.Validate(new Model { Value = "x" });
        var messages = results.SelectMany(r => r.Errors).Select(e => e.Message).ToList();

        Assert.Contains(messages, m => m.Contains("A"));
        Assert.Contains(messages, m => m.Contains("B"));
    }

    private class WhenPerPropertyOverrideValidator : ValidatorBuilder<Model>
    {
        public WhenPerPropertyOverrideValidator()
        {
            this.CascadeMode = CascadeMode.Stop;

            this.When(x => true, () =>
            {
                this.RuleFor(r => r.Value)
                    .SetCascadeMode(CascadeMode.Continue)
                    .Must(v => false).Message("A")
                    .Must(v => false).Message("B");
            });
        }
    }
}
