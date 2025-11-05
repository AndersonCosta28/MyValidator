using Microsoft.Data.Sqlite;

namespace IntegrationTests;

public class DatabaseFixture : IAsyncLifetime
{
    public SqliteConnection Connection { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        this.Connection = new SqliteConnection("Data Source=:memory:");
        await this.Connection.OpenAsync();

        var createCmd = this.Connection.CreateCommand();
        createCmd.CommandText = "CREATE TABLE Users (Id INTEGER PRIMARY KEY, Email TEXT);";
        await createCmd.ExecuteNonQueryAsync();
    }

    public async Task DisposeAsync()
    {
        await this.Connection.CloseAsync();
        this.Connection.Dispose();
    }

    public async Task ResetAsync()
    {
        // Clear rows between tests to ensure a clean slate
        var cmd = this.Connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Users;";
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<SqliteConnection> CreateIsolatedConnectionAsync(string name)
    {
        // Create an isolated in-memory database connection that can be shared by multiple connections
        // using a unique file: URI. Tests can use this to avoid interference.
        var cs = $"Data Source=file:{name}?mode=memory&cache=shared";
        var conn = new SqliteConnection(cs);
        await conn.OpenAsync();

        var createCmd = conn.CreateCommand();
        createCmd.CommandText = "CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY, Email TEXT);";
        await createCmd.ExecuteNonQueryAsync();

        return conn;
    }
}