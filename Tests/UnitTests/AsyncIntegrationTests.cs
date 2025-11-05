using Microsoft.Data.Sqlite;

namespace UnitTests;

public class AsyncIntegrationTests
{
    [Fact]
    public async Task ValidateAsync_WithAsyncRuleAgainstDatabase_Works()
    {
        // Arrange - create in-memory sqlite database and a table
        using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var createCmd = connection.CreateCommand();
        createCmd.CommandText = "CREATE TABLE Users (Id INTEGER PRIMARY KEY, Email TEXT);";
        await createCmd.ExecuteNonQueryAsync();

        var insertCmd = connection.CreateCommand();
        insertCmd.CommandText = "INSERT INTO Users (Email) VALUES ('existing@example.com');";
        await insertCmd.ExecuteNonQueryAsync();

        // Build a simple model and validator that uses MustAsync to check DB uniqueness
        var model = new TestModel { Email = "existing@example.com" };

        var validator = new TestModelValidator(async (email, ct) =>
        {
            // Query sqlite to see if email exists
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(1) FROM Users WHERE Email = $email";
            cmd.Parameters.AddWithValue("$email", email);
            var count = (long)await cmd.ExecuteScalarAsync(ct);
            return count == 0; // valid if not exists
        });

        // Act
        var results = await validator.ValidateAsync(model);

        // Assert - should have an error because email exists
        Assert.Contains(results.SelectMany(r => r.Errors), e => e.Message.Contains("already exists"));
    }

    private class TestModel
    {
        public string Email { get; set; } = string.Empty;
    }

    private class TestModelValidator : ValidatorBuilder<TestModel>
    {
        public TestModelValidator(Func<string, CancellationToken, Task<bool>> asyncCheck) => this.RuleFor(x => x.Email)
            .MustAsync((email, ct) => asyncCheck(email, ct))
            .Message("Email already exists.");
    }
}