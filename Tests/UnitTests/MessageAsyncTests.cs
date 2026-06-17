using System.Threading;
using System.Threading.Tasks;
using Mert1s.MyValidator;

namespace UnitTests;

public class MessageAsyncTests
{
    private class Account
    {
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    // ---- Overload coverage ----------------------------------------------------

    [Fact]
    public async Task MessageAsync_FullOverload_UsesPropertyInstanceAndToken()
    {
        var validator = new FluentValidator(b => b
            .Must(_ => false)
            .MessageAsync((string email, Account acc, CancellationToken _) =>
                Task.FromResult($"'{email}' invalid for age {acc.Age}")));

        var results = await validator.ValidateAsync(new Account { Email = "a@b.com", Age = 30 });

        Assert.Contains(results.SelectMany(r => r.Errors), e => e.Message == "'a@b.com' invalid for age 30");
    }

    [Fact]
    public async Task MessageAsync_PropertyTokenOverload_UsesProperty()
    {
        var validator = new FluentValidator(b => b
            .Must(_ => false)
            .MessageAsync((string email, CancellationToken _) => Task.FromResult($"bad: {email}")));

        var results = await validator.ValidateAsync(new Account { Email = "x@y.com" });

        Assert.Contains(results.SelectMany(r => r.Errors), e => e.Message == "bad: x@y.com");
    }

    [Fact]
    public async Task MessageAsync_TokenOnlyOverload_UsesConstant()
    {
        var validator = new FluentValidator(b => b
            .Must(_ => false)
            .MessageAsync((CancellationToken _) => Task.FromResult("constant async message")));

        var results = await validator.ValidateAsync(new Account());

        Assert.Contains(results.SelectMany(r => r.Errors), e => e.Message == "constant async message");
    }

    [Fact]
    public async Task MessageAsync_PropertyInstanceOverload_UsesBoth()
    {
        var validator = new FluentValidator(b => b
            .Must(_ => false)
            .MessageAsync((string email, Account acc) => Task.FromResult($"{email}/{acc.Age}")));

        var results = await validator.ValidateAsync(new Account { Email = "j@k.com", Age = 42 });

        Assert.Contains(results.SelectMany(r => r.Errors), e => e.Message == "j@k.com/42");
    }

    [Fact]
    public async Task MessageAsync_PropertyOnlyOverload_UsesProperty()
    {
        var validator = new FluentValidator(b => b
            .Must(_ => false)
            .MessageAsync((string email) => Task.FromResult($"only: {email}")));

        var results = await validator.ValidateAsync(new Account { Email = "z@z.com" });

        Assert.Contains(results.SelectMany(r => r.Errors), e => e.Message == "only: z@z.com");
    }

    // ---- Sync path resolves async message (Risk 7 fix) ------------------------

    [Fact]
    public void MessageAsync_ResolvedOnSyncValidate()
    {
        var validator = new FluentValidator(b => b
            .Must(_ => false)
            .MessageAsync((string email) => Task.FromResult($"sync-resolved: {email}")));

        var results = validator.Validate(new Account { Email = "sync@test.com" });

        // The async message func is awaited synchronously; default fallback must NOT appear.
        Assert.Contains(results.SelectMany(r => r.Errors), e => e.Message == "sync-resolved: sync@test.com");
        Assert.DoesNotContain(results.SelectMany(r => r.Errors), e => e.Message == "Erro de validação.");
    }

    [Fact]
    public void MessageAsync_OnAsyncRule_ResolvedOnSyncValidate()
    {
        // MustAsync rule (AsyncValidationRule) combined with MessageAsync, executed via sync Validate().
        var validator = new FluentValidator(b => b
            .MustAsync((_, _) => Task.FromResult(false))
            .MessageAsync((string email) => Task.FromResult($"async-rule sync-msg: {email}")));

        var results = validator.Validate(new Account { Email = "ar@test.com" });

        Assert.Contains(results.SelectMany(r => r.Errors), e => e.Message == "async-rule sync-msg: ar@test.com");
    }

    [Fact]
    public async Task MessageAsync_OnAsyncRule_ResolvedOnValidateAsync()
    {
        var validator = new FluentValidator(b => b
            .MustAsync((_, _) => Task.FromResult(false))
            .MessageAsync((string email) => Task.FromResult($"async-rule async-msg: {email}")));

        var results = await validator.ValidateAsync(new Account { Email = "ar2@test.com" });

        Assert.Contains(results.SelectMany(r => r.Errors), e => e.Message == "async-rule async-msg: ar2@test.com");
    }

    // ---- Precedence and fallback ---------------------------------------------

    [Fact]
    public async Task MessageAsync_TakesPrecedenceOver_Message_Async()
    {
        var validator = new FluentValidator(b => b
            .Must(_ => false)
            .Message("sync message")
            .MessageAsync((string _) => Task.FromResult("async message")));

        var results = await validator.ValidateAsync(new Account());

        Assert.Contains(results.SelectMany(r => r.Errors), e => e.Message == "async message");
        Assert.DoesNotContain(results.SelectMany(r => r.Errors), e => e.Message == "sync message");
    }

    [Fact]
    public void MessageAsync_TakesPrecedenceOver_Message_Sync()
    {
        var validator = new FluentValidator(b => b
            .Must(_ => false)
            .Message("sync message")
            .MessageAsync((string _) => Task.FromResult("async message")));

        var results = validator.Validate(new Account());

        Assert.Contains(results.SelectMany(r => r.Errors), e => e.Message == "async message");
        Assert.DoesNotContain(results.SelectMany(r => r.Errors), e => e.Message == "sync message");
    }

    [Fact]
    public async Task GetErrorMessageAsync_FallsBackTo_SyncMessage_WhenNoAsyncSet()
    {
        var validator = new FluentValidator(b => b
            .Must(_ => false)
            .Message("only sync configured"));

        var results = await validator.ValidateAsync(new Account());

        Assert.Contains(results.SelectMany(r => r.Errors), e => e.Message == "only sync configured");
    }

    // ---- Cancellation token propagation --------------------------------------

    [Fact]
    public async Task MessageAsync_ReceivesCancellationToken_FromValidateAsync()
    {
        var validator = new FluentValidator(b => b
            .Must(_ => false)
            .MessageAsync((string _, CancellationToken ct) =>
                Task.FromResult(ct.IsCancellationRequested ? "CANCELLED" : "ACTIVE")));

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var cancelled = await validator.ValidateAsync(new Account(), cts.Token);
        var active = await validator.ValidateAsync(new Account(), CancellationToken.None);

        Assert.Contains(cancelled.SelectMany(r => r.Errors), e => e.Message == "CANCELLED");
        Assert.Contains(active.SelectMany(r => r.Errors), e => e.Message == "ACTIVE");
    }

    // ---- Targets only the most recently defined rule (Risk 3/4 fix) ----------

    [Fact]
    public async Task MessageAsync_TargetsCurrentRule_NotPreviousRules()
    {
        // Two failing rules; each gets its own async message. They must not bleed.
        var validator = new FluentValidator(b => b
            .Must(_ => false).MessageAsync((string _) => Task.FromResult("first"))
            .Must(_ => false).MessageAsync((string _) => Task.FromResult("second")));

        var results = await validator.ValidateAsync(new Account());
        var messages = results.SelectMany(r => r.Errors).Select(e => e.Message).ToList();

        Assert.Contains("first", messages);
        Assert.Contains("second", messages);
    }

    // ---- Helper validator that exposes a fluent rule builder ------------------

    private class FluentValidator : ValidatorBuilder<Account>
    {
        public FluentValidator(Func<RuleBuilder<Account, string>, RuleBuilder<Account, string>> configure) =>
            configure(this.RuleFor(x => x.Email));
    }
}
