using System.Threading;
using System.Threading.Tasks;
using Mert1s.MyValidator;

namespace UnitTests;

public class ConditionalValidationTests
{
    public class MaioridadePenal
    {
        public string Pais { get; set; } = string.Empty;
        public int IdadeDeMaioridade { get; set; }
    }

    [Fact]
    public void When_AppliesRulesConditionally_Grouped()
    {
        var validator = new TestWhenValidator();

        var brasilResult = validator.Validate(new MaioridadePenal { Pais = "Brasil", IdadeDeMaioridade = 18 });
        var euaResult = validator.Validate(new MaioridadePenal { Pais = "EUA", IdadeDeMaioridade = 21 });
        var canadaResult = validator.Validate(new MaioridadePenal { Pais = "Canadá", IdadeDeMaioridade = 19 });
        var otherResult = validator.Validate(new MaioridadePenal { Pais = "México", IdadeDeMaioridade = 18 });

        Assert.Empty(brasilResult.SelectMany(r => r.Errors));
        Assert.Empty(euaResult.SelectMany(r => r.Errors));
        Assert.Empty(canadaResult.SelectMany(r => r.Errors));
        Assert.Empty(otherResult.SelectMany(r => r.Errors));
    }

    [Fact]
    public void When_FailsValidationWhenConditionsNotMet()
    {
        var validator = new TestWhenValidator();

        var brasilResult = validator.Validate(new MaioridadePenal { Pais = "Brasil", IdadeDeMaioridade = 17 });
        var euaResult = validator.Validate(new MaioridadePenal { Pais = "EUA", IdadeDeMaioridade = 20 });
        var canadaResult = validator.Validate(new MaioridadePenal { Pais = "Canadá", IdadeDeMaioridade = 18 });

        Assert.Contains(brasilResult.SelectMany(r => r.Errors), e => e.Message.Contains("greater than or equal to 18"));
        Assert.Contains(euaResult.SelectMany(r => r.Errors), e => e.Message.Contains("greater than or equal to 21"));
        Assert.Contains(canadaResult.SelectMany(r => r.Errors), e => e.Message.Contains("is not equal to 19"));
    }

    [Fact]
    public void When_Grouped_SetCascadeMode_AppliesStop()
    {
        var validator = new GroupedCascadeValidator();

        var result = validator.Validate(new MaioridadePenal { Pais = "Brasil", IdadeDeMaioridade = 17 });
        var messages = result.SelectMany(r => r.Errors).Select(e => e.Message).ToList();

        Assert.Single(messages);
        Assert.Contains(messages, m => m.Contains("A"));
    }

    [Fact]
    public void When_Chained_SetCascadeMode_AppliesStop()
    {
        var validator = new ChainedCascadeValidator();

        var result = validator.Validate(new MaioridadePenal { Pais = "Brasil", IdadeDeMaioridade = 17 });
        var messages = result.SelectMany(r => r.Errors).Select(e => e.Message).ToList();

        Assert.Single(messages);
        Assert.Contains(messages, m => m.Contains("A"));
    }

    [Fact]
    public async Task WhenAsync_GroupedAndChained_Works()
    {
        var grouped = new GroupedAsyncWhenValidator();
        var chained = new ChainedAsyncWhenValidator();

        var br = new MaioridadePenal { Pais = "Brasil", IdadeDeMaioridade = 17 };

        var gSync = grouped.Validate(br);
        var cSync = chained.Validate(br);
        Assert.Equal(gSync.SelectMany(r => r.Errors).Count(), cSync.SelectMany(r => r.Errors).Count());

        var gAsync = await grouped.ValidateAsync(br);
        var cAsync = await chained.ValidateAsync(br);
        Assert.Equal(gAsync.SelectMany(r => r.Errors).Count(), cAsync.SelectMany(r => r.Errors).Count());
    }

    private class TestWhenValidator : ValidatorBuilder<MaioridadePenal>
    {
        public TestWhenValidator()
        {
            this.When(x => x.Pais == "Brasil", () =>
            {
                this.RuleFor(x => x.IdadeDeMaioridade)
                    .GreaterThanOrEqual(18);
            });

            this.When(x => x.Pais == "EUA", () =>
            {
                this.RuleFor(x => x.IdadeDeMaioridade)
                    .GreaterThanOrEqual(21);
            });

            this.When(x => x.Pais == "Canadá", () =>
            {
                this.RuleFor(x => x.IdadeDeMaioridade)
                    .IsEqual(19);
            });
        }
    }

    private class GroupedCascadeValidator : ValidatorBuilder<MaioridadePenal>
    {
        public GroupedCascadeValidator()
        {
            this.When(x => x.Pais == "Brasil", () =>
            {
                this.RuleFor(x => x.IdadeDeMaioridade)
                    .Must(v => false).Message("A")
                    .Must(v => false).Message("B");
            }).SetCascadeMode(CascadeMode.Stop);
        }
    }

    private class ChainedCascadeValidator : ValidatorBuilder<MaioridadePenal>
    {
        public ChainedCascadeValidator()
        {
            this.RuleFor(x => x.IdadeDeMaioridade)
                .Must(v => false).Message("A")
                .Must(v => false).Message("B")
                .When(x => x.Pais == "Brasil")
                .SetCascadeMode(CascadeMode.Stop);
        }
    }

    private class GroupedAsyncWhenValidator : ValidatorBuilder<MaioridadePenal>
    {
        public GroupedAsyncWhenValidator()
        {
            this.WhenAsync((m, ct) => Task.FromResult(m.Pais == "Brasil"), () =>
            {
                this.RuleFor(x => x.IdadeDeMaioridade).GreaterThanOrEqual(18).Message("BR");
            });
        }
    }

    private class ChainedAsyncWhenValidator : ValidatorBuilder<MaioridadePenal>
    {
        public ChainedAsyncWhenValidator()
        {
            this.RuleFor(x => x.IdadeDeMaioridade)
                .GreaterThanOrEqual(18)
                .When(async m => await Task.FromResult(m.Pais == "Brasil"));
        }
    }

    [Fact]
    public async Task WhenAsync_Validation_CanBeCancelled()
    {
        var validator = new CancellationWhenValidator();
        var instance = new MaioridadePenal { Pais = "Brasil", IdadeDeMaioridade = 18 };

        using var cts = new CancellationTokenSource();
        cts.Cancel(); // cancel before calling ValidateAsync

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await validator.ValidateAsync(instance, cts.Token));
    }

    private class CancellationWhenValidator : ValidatorBuilder<MaioridadePenal>
    {
        public CancellationWhenValidator()
        {
            // Predicate delays and observes cancellation token; when cancelled it will throw
            this.WhenAsync(async (m, ct) =>
            {
                await Task.Delay(1000, ct).ConfigureAwait(false);
                return true;
            }, () =>
            {
                this.RuleFor(x => x.IdadeDeMaioridade).GreaterThanOrEqual(18);
            });
        }
    }
}
