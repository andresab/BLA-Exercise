using BookCatalog.Domain.Entities;
using BookCatalog.Domain.Repositories;
using Microsoft.Data.SqlClient;

namespace BookCatalog.Infrastructure.Data;

public sealed class BookRepository : IBookRepository
{
    private readonly SqlConnection _connection;

    public BookRepository(SqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, Title, Author, ISBN, PublishedYear, CreatedAt, UpdatedAt
            FROM Books
            ORDER BY Title;
            """;
        await using var command = new SqlCommand(sql, _connection);
        await OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var books = new List<Book>();
        while (await reader.ReadAsync(cancellationToken))
        {
            books.Add(MapBook(reader));
        }

        return books;
    }

    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, Title, Author, ISBN, PublishedYear, CreatedAt, UpdatedAt
            FROM Books
            WHERE Id = @Id;
            """;
        await using var command = new SqlCommand(sql, _connection);
        command.Parameters.AddWithValue("@Id", id);
        await OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? MapBook(reader) : null;
    }

    public async Task<Book> CreateAsync(Book book, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO Books (Id, Title, Author, ISBN, PublishedYear, CreatedAt, UpdatedAt)
            VALUES (@Id, @Title, @Author, @ISBN, @PublishedYear, @CreatedAt, @UpdatedAt);
            SELECT Id, Title, Author, ISBN, PublishedYear, CreatedAt, UpdatedAt
            FROM Books
            WHERE Id = @Id;
            """;
        await using var command = CreateBookCommand(sql, book);
        await OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return MapBook(reader);
        }

        throw new InvalidOperationException("Book insert did not return a row.");
    }

    public async Task<Book> UpdateAsync(Book book, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE Books
            SET Title = @Title,
                Author = @Author,
                ISBN = @ISBN,
                PublishedYear = @PublishedYear,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id;
            SELECT Id, Title, Author, ISBN, PublishedYear, CreatedAt, UpdatedAt
            FROM Books
            WHERE Id = @Id;
            """;
        await using var command = CreateBookCommand(sql, book);
        await OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return MapBook(reader);
        }

        throw new InvalidOperationException("Book update did not return a row.");
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM Books WHERE Id = @Id;";
        await using var command = new SqlCommand(sql, _connection);
        command.Parameters.AddWithValue("@Id", id);
        await OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<bool> ExistsByIsbnAsync(string isbn, CancellationToken cancellationToken)
    {
        const string sql = "SELECT COUNT(1) FROM Books WHERE ISBN = @ISBN;";
        await using var command = new SqlCommand(sql, _connection);
        command.Parameters.AddWithValue("@ISBN", isbn);
        await OpenAsync(cancellationToken);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result) > 0;
    }

    private async Task OpenAsync(CancellationToken cancellationToken)
    {
        if (_connection.State != System.Data.ConnectionState.Open)
        {
            await _connection.OpenAsync(cancellationToken);
        }
    }

    private SqlCommand CreateBookCommand(string sql, Book book)
    {
        var command = new SqlCommand(sql, _connection);
        command.Parameters.AddWithValue("@Id", book.Id);
        command.Parameters.AddWithValue("@Title", book.Title);
        command.Parameters.AddWithValue("@Author", book.Author);
        command.Parameters.AddWithValue("@ISBN", book.ISBN);
        command.Parameters.AddWithValue("@PublishedYear", book.PublishedYear);
        command.Parameters.AddWithValue("@CreatedAt", book.CreatedAt);
        command.Parameters.AddWithValue("@UpdatedAt", book.UpdatedAt);
        return command;
    }

    private static Book MapBook(SqlDataReader reader) => new(
        reader.GetGuid(0),
        reader.GetString(1),
        reader.GetString(2),
        reader.GetString(3),
        reader.GetInt32(4),
        reader.GetDateTime(5),
        reader.GetDateTime(6));
}
