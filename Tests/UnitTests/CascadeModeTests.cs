using System.Threading;
using System.Threading.Tasks;
using Mert1s.MyValidator;

namespace UnitTests;

public class CascadeModeTests
{
    private class Model { public string Value { get; set; } = string.Empty; public string Other { get; set; } = string.Empty; }

    [Fact]
    public void DefaultContinue_AllRulesExecuted()
    {
        var validator = new DefaultValidator();
        var results = validator.Validate(new Model { Value = "x" });
        var messages = results.SelectMany(r => r.Errors).Select(e => e.Message).ToList();

        Assert.Contains(messages, m => m.Contains("A"));
        Assert.Contains(messages, m => m.Contains("B"));
    }

    private class DefaultValidator : ValidatorBuilder<Model>
    {
        public DefaultValidator()
        {
            this.RuleFor(x => x.Value)
                .Must(v => false).Message("A")
                .Must(v => false).Message("B");
        }
    }

    [Fact]
    public void GlobalStop_StopsAfterFirstFailure()
    {
        var validator = new GlobalStopValidator();
        var results = validator.Validate(new Model { Value = "x" });
        var messages = results.SelectMany(r => r.Errors).Select(e => e.Message).ToList();

        Assert.Single(messages);
        Assert.Contains(messages, m => m.Contains("A"));
    }

    private class GlobalStopValidator : ValidatorBuilder<Model>
    {
        public GlobalStopValidator()
        {
            this.CascadeMode = CascadeMode.Stop;

            this.RuleFor(x => x.Value)
                .Must(v => false).Message("A")
                .Must(v => false).Message("B");
        }
    }

    [Fact]
    public void PerPropertyOverride_OverridesGlobalStop()
    {
        var validator = new PerPropertyOverrideValidator();
        var results = validator.Validate(new Model { Value = "x" });
        var messages = results.SelectMany(r => r.Errors).Select(e => e.Message).ToList();

        // property has local override to Continue, so both rules execute
        Assert.Contains(messages, m => m.Contains("A"));
        Assert.Contains(messages, m => m.Contains("B"));
    }

    private class PerPropertyOverrideValidator : ValidatorBuilder<Model>
    {
        public PerPropertyOverrideValidator()
        {
            this.CascadeMode = CascadeMode.Stop;

            this.RuleFor(x => x.Value)
                .SetCascadeMode(CascadeMode.Continue)
                .Must(v => false).Message("A")
                .Must(v => false).Message("B");
        }
    }

    [Fact]
    public void MixedProperties_GlobalStop_AppliesPerProperty()
    {
        var validator = new MixedPropertiesValidator();
        var results = validator.Validate(new Model { Value = "x", Other = "y" });
        var messages = results.SelectMany(r => r.Errors).Select(e => e.Message).ToList();

        // For Value (no override) only first rule should run
        Assert.Contains(messages, m => m.Contains("A1"));
        Assert.DoesNotContain(messages, m => m.Contains("A2"));

        // For Other (no override but different property) both rules should run if global Continue; however global is Stop
        // We added Other rules without override so Stop applies per property; both rules were added and since first fails, second skipped
        // To illustrate per-property behavior we assert B1 present and B2 not present
        Assert.Contains(messages, m => m.Contains("B1"));
        Assert.DoesNotContain(messages, m => m.Contains("B2"));
    }

    private class MixedPropertiesValidator : ValidatorBuilder<Model>
    {
        public MixedPropertiesValidator()
        {
            this.CascadeMode = CascadeMode.Stop;

            this.RuleFor(x => x.Value)
                .Must(v => false).Message("A1")
                .Must(v => false).Message("A2");

            this.RuleFor(x => x.Other)
                .Must(v => false).Message("B1")
                .Must(v => false).Message("B2");
        }
    }

    [Fact]
    public async Task AsyncRules_RespectCascadeMode_Stop()
    {
        var validator = new AsyncStopValidator();
        var results = await validator.ValidateAsync(new Model { Value = "x" });
        var messages = results.SelectMany(r => r.Errors).Select(e => e.Message).ToList();

        Assert.Single(messages);
        Assert.Contains(messages, m => m.Contains("A"));
    }

    private class AsyncStopValidator : ValidatorBuilder<Model>
    {
        public AsyncStopValidator()
        {
            this.CascadeMode = CascadeMode.Stop;

            this.RuleFor(x => x.Value)
                .MustAsync((v, ct) => Task.FromResult(false)).Message("A")
                .MustAsync((v, ct) => Task.FromResult(false)).Message("B");
        }
    }
}
