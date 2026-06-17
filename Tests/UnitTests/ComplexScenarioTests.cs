using System.Threading;
using System.Threading.Tasks;
using Mert1s.MyValidator;

namespace UnitTests;

/// <summary>
/// End-to-end scenarios that combine several features at once: async rules,
/// async messages, conditional (When) rules, nested/collection validators and
/// cascade modes — exercised through both the sync and async validation paths.
/// </summary>
public class ComplexScenarioTests
{
    private class Order
    {
        public string Country { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string? Coupon { get; set; }
        public List<OrderLine> Lines { get; set; } = [];
    }

    private class OrderLine
    {
        public string Sku { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    private class OrderValidator : ValidatorBuilder<Order>
    {
        public OrderValidator()
        {
            // Async rule + async message (simulates an async uniqueness/availability check).
            this.RuleFor(x => x.Coupon!)
                .MustAsync((coupon, _) => Task.FromResult(string.IsNullOrEmpty(coupon) || coupon == "VALID"))
                .MessageAsync((string coupon, CancellationToken _) => Task.FromResult($"Coupon '{coupon}' is not valid"));

            // Conditional sync rule + async message: minimum only enforced for BR.
            this.RuleFor(x => x.Total)
                .Must(t => t >= 10m)
                .MessageAsync((decimal t, Order o, CancellationToken _) =>
                    Task.FromResult($"Total {t} below minimum for {o.Country}"))
                .When(o => o.Country == "BR");

            // Nested collection validator with its own async message.
            this.RulesFor(x => x.Lines)
                .SetValidator(new OrderLineValidator());
        }
    }

    private class OrderLineValidator : ValidatorBuilder<OrderLine>
    {
        public OrderLineValidator() =>
            this.RuleFor(x => x.Quantity)
                .Must(q => q > 0)
                .MessageAsync((int _, OrderLine line, CancellationToken _) =>
                    Task.FromResult($"SKU {line.Sku}: quantity must be positive"));
    }

    [Fact]
    public async Task FullInvalidOrder_Async_ProducesAllErrorsWithCorrectPaths()
    {
        var order = new Order
        {
            Country = "BR",
            Total = 5m,
            Coupon = "BAD",
            Lines =
            [
                new OrderLine { Sku = "A", Quantity = 0 },
                new OrderLine { Sku = "B", Quantity = 2 }
            ]
        };

        var results = await new OrderValidator().ValidateAsync(order);
        var errors = results.SelectMany(r => r.Errors).ToList();

        Assert.Contains(errors, e => e.Message == "Coupon 'BAD' is not valid");
        Assert.Contains(errors, e => e.Message == "Total 5 below minimum for BR");
        Assert.Contains(errors, e => e.Message == "SKU A: quantity must be positive" && e.Path == "Lines[0].Quantity");
        // The second line is valid — no error for index 1.
        Assert.DoesNotContain(errors, e => e.Path == "Lines[1].Quantity");
    }

    [Fact]
    public void FullInvalidOrder_Sync_ResolvesAsyncMessagesAcrossAllRuleTypes()
    {
        var order = new Order
        {
            Country = "BR",
            Total = 5m,
            Coupon = "BAD",
            Lines = [new OrderLine { Sku = "A", Quantity = 0 }]
        };

        // Sync Validate() must drive async conditions AND async messages through
        // GetAwaiter().GetResult() for the MustAsync rule, the Must rule and the nested rule.
        var results = new OrderValidator().Validate(order);
        var errors = results.SelectMany(r => r.Errors).ToList();

        Assert.Contains(errors, e => e.Message == "Coupon 'BAD' is not valid");
        Assert.Contains(errors, e => e.Message == "Total 5 below minimum for BR");
        Assert.Contains(errors, e => e.Message == "SKU A: quantity must be positive");
        Assert.DoesNotContain(errors, e => e.Message == "Erro de validação.");
    }

    [Fact]
    public async Task ConditionalRuleSkipped_WhenCountryNotBr()
    {
        var order = new Order
        {
            Country = "US",
            Total = 5m,            // below 10, but rule only applies to BR
            Coupon = "VALID",
            Lines = [new OrderLine { Sku = "A", Quantity = 1 }]
        };

        var results = await new OrderValidator().ValidateAsync(order);

        Assert.Empty(results.SelectMany(r => r.Errors));
    }

    // ---- Cascade + async rules + async messages on the same property ----------

    private class CascadeAsyncValidator : ValidatorBuilder<Order>
    {
        public CascadeAsyncValidator()
        {
            this.CascadeMode = CascadeMode.Stop;

            this.RuleFor(x => x.Country)
                .MustAsync((_, _) => Task.FromResult(false))
                .MessageAsync((string _) => Task.FromResult("first failure"))
                .MustAsync((_, _) => Task.FromResult(false))
                .MessageAsync((string _) => Task.FromResult("second failure"));
        }
    }

    [Fact]
    public async Task CascadeStop_WithAsyncRulesAndMessages_StopsAfterFirstFailure()
    {
        var results = await new CascadeAsyncValidator().ValidateAsync(new Order { Country = "X" });
        var messages = results.SelectMany(r => r.Errors).Select(e => e.Message).ToList();

        Assert.Single(messages);
        Assert.Equal("first failure", messages[0]);
    }

    [Fact]
    public void CascadeStop_WithAsyncRulesAndMessages_StopsAfterFirstFailure_Sync()
    {
        var results = new CascadeAsyncValidator().Validate(new Order { Country = "X" });
        var messages = results.SelectMany(r => r.Errors).Select(e => e.Message).ToList();

        Assert.Single(messages);
        Assert.Equal("first failure", messages[0]);
    }

    // ---- Chained WhenAsync (RuleBuilder) gating an async rule -----------------

    private class GatedAsyncValidator : ValidatorBuilder<Order>
    {
        public GatedAsyncValidator() =>
            this.RuleFor(x => x.Total)
                .Must(t => t >= 100m)
                .MessageAsync((decimal t, CancellationToken _) => Task.FromResult($"premium minimum not met: {t}"))
                .WhenAsync((o, _) => Task.FromResult(o.Country == "BR"));

    }

    [Fact]
    public async Task ChainedWhenAsync_RunsRuleOnlyWhenPredicateTrue()
    {
        var br = await new GatedAsyncValidator().ValidateAsync(new Order { Country = "BR", Total = 50m });
        var us = await new GatedAsyncValidator().ValidateAsync(new Order { Country = "US", Total = 50m });

        Assert.Contains(br.SelectMany(r => r.Errors), e => e.Message == "premium minimum not met: 50");
        Assert.Empty(us.SelectMany(r => r.Errors));
    }
}
