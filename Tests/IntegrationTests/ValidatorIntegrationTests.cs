namespace IntegrationTests;

[Collection("IntegrationCollection")]
public class ValidatorIntegrationTests
{
    private readonly DatabaseFixture _fixture;

    public ValidatorIntegrationTests(DatabaseFixture fixture) => this._fixture = fixture;

    [Theory]
    [InlineData("existing@example.com", false)]
    [InlineData("new@example.com", true)]
    public async Task ValidateAsync_EmailUniqueness_Scenarios(string email, bool expectValid)
    {
        // Arrange
        var connection = this._fixture.Connection;

        // Ensure the table has one existing email
        await this._fixture.ResetAsync();

        var insertCmd = connection.CreateCommand();
        insertCmd.CommandText = "INSERT INTO Users (Email) VALUES ('existing@example.com');";
        await insertCmd.ExecuteNonQueryAsync();

        var model = new TestModel { Email = email };
        var validator = new TestModelValidator((Func<string, CancellationToken, Task<bool>>)((e, ct) =>
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(1) FROM Users WHERE Email = $email";
            cmd.Parameters.AddWithValue("$email", e);
            var count = (long)cmd.ExecuteScalar();
            return Task.FromResult(count == 0);
        }));

        // Act
        var results = await validator.ValidateAsync(model, CancellationToken.None);
        var hasErrors = results.SelectMany(r => r.Errors).Any();

        // Assert
        Assert.Equal(!expectValid, hasErrors);
    }

    [Fact]
    public async Task ValidateAsync_MultipleRecords_AllInvalid()
    {
        var connection = this._fixture.Connection;

        await this._fixture.ResetAsync();

        var insertCmd = connection.CreateCommand();
        insertCmd.CommandText = "INSERT INTO Users (Email) VALUES ('existing@example.com'), ('other@example.com');";
        await insertCmd.ExecuteNonQueryAsync();

        var model = new TestModel { Email = "existing@example.com" };
        var validator = new TestModelValidator((Func<string, CancellationToken, Task<bool>>)((e, ct) =>
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(1) FROM Users WHERE Email = $email";
            cmd.Parameters.AddWithValue("$email", e);
            var count = (long)cmd.ExecuteScalar();
            return Task.FromResult(count == 0);
        }));

        var results = await validator.ValidateAsync(model);
        var errors = results.SelectMany(r => r.Errors).ToList();

        Assert.NotEmpty(errors);
        Assert.Contains(errors, e => e.Message.Contains("already exists") || e.Message.Contains("Email already exists"));
    }

    [Fact]
    public async Task ValidateAsync_WithCancellationToken_Cancels()
    {
        var connection = this._fixture.Connection;

        await this._fixture.ResetAsync();

        var insertCmd = connection.CreateCommand();
        insertCmd.CommandText = "INSERT INTO Users (Email) VALUES ('existing@example.com');";
        await insertCmd.ExecuteNonQueryAsync();

        var model = new TestModel { Email = "existing@example.com" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var validator = new TestModelValidator((Func<string, CancellationToken, Task<bool>>)((e, ct) =>
        {
            Task.Delay(1000, ct).GetAwaiter().GetResult();
            return Task.FromResult(true);
        }));

        await Assert.ThrowsAsync<TaskCanceledException>(async () => await validator.ValidateAsync(model, cts.Token));
    }

    [Fact]
    public async Task ValidateAsync_NestedValidator_PropagatesAsync()
    {
        // Use an isolated in-memory database for this test
        var name = Guid.NewGuid().ToString("N");
        await using var conn = await this._fixture.CreateIsolatedConnectionAsync(name);

        // Insert a record that will make the child validator fail
        var insert = conn.CreateCommand();
        insert.CommandText = "INSERT INTO Users (Email) VALUES ('child@example.com');";
        await insert.ExecuteNonQueryAsync();

        // Parent model contains a nested child
        var parent = new ParentModel { Child = new ChildModel { Email = "child@example.com" } };

        // Child validator checks DB asynchronously
        var childValidator = new ChildValidator(conn);

        // Parent validator uses SetValidatorAsync to set nested async validator
        var parentValidator = new ParentValidator(childValidator);

        // Act
        var results = await parentValidator.ValidateAsync(parent);
        var errors = results.SelectMany(r => r.Errors).ToList();

        // Assert - child validator should produce an error which bubbles up
        Assert.NotEmpty(errors);
        Assert.Contains(errors, e => e.Message.Contains("already exists") || e.Message.Contains("Email already exists"));

        await conn.CloseAsync();
    }

    [Fact]
    public async Task ValidateAsync_NestedValidator_CancellationPropagates()
    {
        var name = Guid.NewGuid().ToString("N");
        await using var conn = await this._fixture.CreateIsolatedConnectionAsync(name);

        var parent = new ParentModel { Child = new ChildModel { Email = "any@example.com" } };

        // Child validator that delays and supports cancellation
        var childValidator = new DelayingChildValidator();
        var parentValidator = new ParentValidator(childValidator);

        var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(async () => await parentValidator.ValidateAsync(parent, cts.Token));
        await conn.CloseAsync();
    }

    private class TestModel
    {
        public string Email { get; set; } = string.Empty;
    }

    private class TestModelValidator : ValidatorBuilder<TestModel>
    {
        public TestModelValidator(Func<string, CancellationToken, Task<bool>> asyncCheck) => this.RuleFor(x => x.Email)
        .MustAsync((Func<string, CancellationToken, Task<bool>>)((email, ct) => asyncCheck(email, ct)))
        .Message("Email already exists.");
    }

    private class ParentModel
    {
        public ChildModel Child { get; set; } = new ChildModel();
    }

    private class ChildModel
    {
        public string Email { get; set; } = string.Empty;
    }

    private class ChildValidator : ValidatorBuilder<ChildModel>
    {
        public ChildValidator(Microsoft.Data.Sqlite.SqliteConnection conn)
        {
            this.RuleFor(x => x.Email)
            .MustAsync((Func<string, CancellationToken, Task<bool>>)(async (email, ct) =>
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(1) FROM Users WHERE Email = $email";
                cmd.Parameters.AddWithValue("$email", email);
                var count = (long)await cmd.ExecuteScalarAsync(ct);
                return count == 0;
            }))
            .Message("Email already exists.");
        }
    }

    private class DelayingChildValidator : ValidatorBuilder<ChildModel>
    {
        public DelayingChildValidator() => this.RuleFor(x => x.Email)
        .MustAsync((Func<string, CancellationToken, Task<bool>>)(async (email, ct) =>
        {
            await Task.Delay(1000, ct);
            return true;
        }))
        .Message("Delayed check");
    }

    private class ParentValidator : ValidatorBuilder<ParentModel>
    {
        public ParentValidator(ValidatorBuilder<ChildModel> childValidator)
        {
            this.RuleFor(x => x.Child)
            .SetValidatorAsync(childValidator);
        }
    }
}