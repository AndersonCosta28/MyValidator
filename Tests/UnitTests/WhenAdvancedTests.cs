using System.Threading;
using System.Threading.Tasks;
using Mert1s.MyValidator;

namespace UnitTests;

public class WhenAdvancedTests
{
    private class Model { public string Pais { get; set; } = string.Empty; public int Idade { get; set; } }

    [Fact]
    public void GroupedWhen_Equals_ChainedWhen()
    {
        var grouped = new GroupedWhenValidator();
        var chained = new ChainedWhenValidator();

        var br = new Model { Pais = "Brasil", Idade = 17 };
        var us = new Model { Pais = "EUA", Idade = 20 };

        var gBr = grouped.Validate(br);
        var cBr = chained.Validate(br);
        Assert.Equal(gBr.SelectMany(r => r.Errors).Count(), cBr.SelectMany(r => r.Errors).Count());

        var gUs = grouped.Validate(us);
        var cUs = chained.Validate(us);
        Assert.Equal(gUs.SelectMany(r => r.Errors).Count(), cUs.SelectMany(r => r.Errors).Count());
    }

    private class GroupedWhenValidator : ValidatorBuilder<Model>
    {
        public GroupedWhenValidator()
        {
            this.When(x => x.Pais == "Brasil", () =>
            {
                this.RuleFor(x => x.Idade).GreaterThanOrEqual(18).Message("BR");
            });

            this.When(x => x.Pais == "EUA", () =>
            {
                this.RuleFor(x => x.Idade).GreaterThanOrEqual(21).Message("US");
            });
        }
    }

    private class ChainedWhenValidator : ValidatorBuilder<Model>
    {
        public ChainedWhenValidator()
        {
            this.RuleFor(x => x.Idade)
                .GreaterThanOrEqual(18)
                .When(x => x.Pais == "Brasil");

            this.RuleFor(x => x.Idade)
                .GreaterThanOrEqual(21)
                .When(x => x.Pais == "EUA");
        }
    }

    [Fact]
    public async Task WhenAsync_GroupedAndChained_Works()
    {
        var grouped = new GroupedAsyncWhenValidator();
        var chained = new ChainedAsyncWhenValidator();

        var br = new Model { Pais = "Brasil", Idade = 17 };
        var brGrouped = grouped.Validate(br);
        var brChained = chained.Validate(br);
        Assert.Equal(brGrouped.SelectMany(r => r.Errors).Count(), brChained.SelectMany(r => r.Errors).Count());

        // Validate async path
        var brGroupedAsync = await grouped.ValidateAsync(br);
        var brChainedAsync = await chained.ValidateAsync(br);
        Assert.Equal(brGroupedAsync.SelectMany(r => r.Errors).Count(), brChainedAsync.SelectMany(r => r.Errors).Count());
    }

    private class GroupedAsyncWhenValidator : ValidatorBuilder<Model>
    {
        public GroupedAsyncWhenValidator()
        {
            this.WhenAsync((m, ct) => Task.FromResult(m.Pais == "Brasil"), () =>
            {
                this.RuleFor(x => x.Idade).GreaterThanOrEqual(18).Message("BR");
            });
        }
    }

    private class ChainedAsyncWhenValidator : ValidatorBuilder<Model>
    {
        public ChainedAsyncWhenValidator()
        {
            this.RuleFor(x => x.Idade)
                .GreaterThanOrEqual(18)
                .When(async m => await Task.FromResult(m.Pais == "Brasil"));
        }
    }
}
