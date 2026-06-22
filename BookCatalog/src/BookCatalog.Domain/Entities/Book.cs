using System.Text.RegularExpressions;
using BookCatalog.Domain.Exceptions;

namespace BookCatalog.Domain.Entities;

public sealed record Book
{
    public const int MaxTitleLength = 200;
    public const int MaxAuthorLength = 150;
    public const int MinPublishedYear = 1450;
    public const string IsbnPattern = @"^\d{9}[\dX]$|^\d{13}$";

    private static readonly Regex IsbnRegex = new(IsbnPattern, RegexOptions.Compiled);

    public Book(
        Guid id,
        string title,
        string author,
        string isbn,
        int publishedYear,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Validate(title, author, isbn, publishedYear);

        Id = id;
        Title = title.Trim();
        Author = author.Trim();
        ISBN = isbn.Trim();
        PublishedYear = publishedYear;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; }
    public string Title { get; }
    public string Author { get; }
    public string ISBN { get; }
    public int PublishedYear { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }

    public static Book Create(string title, string author, string isbn, int publishedYear)
    {
        var now = DateTime.UtcNow;
        return new Book(Guid.NewGuid(), title, author, isbn, publishedYear, now, now);
    }

    public Book Update(string title, string author, string isbn, int publishedYear) =>
        new(Id, title, author, isbn, publishedYear, CreatedAt, DateTime.UtcNow);

    private static void Validate(string title, string author, string isbn, int publishedYear)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ValidationException("Title is required.");
        }

        if (title.Length > MaxTitleLength)
        {
            throw new ValidationException("Title must be 200 characters or fewer.");
        }

        if (string.IsNullOrWhiteSpace(author))
        {
            throw new ValidationException("Author is required.");
        }

        if (author.Length > MaxAuthorLength)
        {
            throw new ValidationException("Author must be 150 characters or fewer.");
        }

        if (string.IsNullOrWhiteSpace(isbn) || !IsbnRegex.IsMatch(isbn.Trim()))
        {
            throw new ValidationException("ISBN must be ISBN-10 or ISBN-13 format.");
        }

        if (publishedYear < MinPublishedYear || publishedYear > DateTime.UtcNow.Year)
        {
            throw new ValidationException($"Published year must be between {MinPublishedYear} and {DateTime.UtcNow.Year}.");
        }
    }
}
