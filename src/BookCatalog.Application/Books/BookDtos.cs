namespace BookCatalog.Application.Books;

public sealed record CreateBookDto(string Title, string Author, string ISBN, int PublishedYear);

public sealed record UpdateBookDto(string? Title, string? Author, string? ISBN, int? PublishedYear);

public sealed record BookResponseDto(
    Guid Id,
    string Title,
    string Author,
    string ISBN,
    int PublishedYear,
    DateTime CreatedAt,
    DateTime UpdatedAt);
