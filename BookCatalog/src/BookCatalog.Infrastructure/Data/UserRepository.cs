using BookCatalog.Domain.Entities;
using BookCatalog.Domain.Repositories;
using Microsoft.Data.SqlClient;

namespace BookCatalog.Infrastructure.Data;

public sealed class UserRepository : IUserRepository
{
    private readonly SqlConnection _connection;

    public UserRepository(SqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, Username, Email, PasswordHash, CreatedAt
            FROM Users
            WHERE Email = @Email;
            """;
        await using var command = new SqlCommand(sql, _connection);
        command.Parameters.AddWithValue("@Email", email.Trim().ToLowerInvariant());
        await OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? MapUser(reader) : null;
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO Users (Id, Username, Email, PasswordHash, CreatedAt)
            VALUES (@Id, @Username, @Email, @PasswordHash, @CreatedAt);
            SELECT Id, Username, Email, PasswordHash, CreatedAt
            FROM Users
            WHERE Id = @Id;
            """;
        await using var command = new SqlCommand(sql, _connection);
        command.Parameters.AddWithValue("@Id", user.Id);
        command.Parameters.AddWithValue("@Username", user.Username);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
        await OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return MapUser(reader);
        }

        throw new InvalidOperationException("User insert did not return a row.");
    }

    private async Task OpenAsync(CancellationToken cancellationToken)
    {
        if (_connection.State != System.Data.ConnectionState.Open)
        {
            await _connection.OpenAsync(cancellationToken);
        }
    }

    private static User MapUser(SqlDataReader reader) => new(
        reader.GetGuid(0),
        reader.GetString(1),
        reader.GetString(2),
        reader.GetString(3),
        reader.GetDateTime(4));
}
