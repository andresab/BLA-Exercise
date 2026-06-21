using BookCatalog.Domain.Entities;
using BookCatalog.Domain.Exceptions;
using BookCatalog.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace BookCatalog.Application.Books;

public sealed class BookService(IBookRepository bookRepository, ILogger<BookService> logger) : IBookService
{
    public async Task<IEnumerable<BookResponseDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching all books");
        var books = await bookRepository.GetAllAsync(cancellationToken);
        return [.. books.Select(ToDto)];
    }

    public async Task<BookResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching book {BookId}", id);
        var book = await bookRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Book '{id}' was not found.");
        return ToDto(book);
    }

    public async Task<BookResponseDto> CreateAsync(CreateBookDto dto, CancellationToken cancellationToken)
    {
        var book = Book.Create(dto.Title, dto.Author, dto.ISBN, dto.PublishedYear);
        if (await bookRepository.ExistsByIsbnAsync(book.ISBN, cancellationToken))
        {
            throw new ValidationException($"A book with ISBN '{book.ISBN}' already exists.");
        }

        var created = await bookRepository.CreateAsync(book, cancellationToken);
        logger.LogInformation("Created book {BookId}", created.Id);
        return ToDto(created);
    }

    public async Task<BookResponseDto> UpdateAsync(Guid id, UpdateBookDto dto, CancellationToken cancellationToken)
    {
        var existing = await bookRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Book '{id}' was not found.");

        var title = dto.Title ?? existing.Title;
        var author = dto.Author ?? existing.Author;
        var isbn = dto.ISBN ?? existing.ISBN;
        var publishedYear = dto.PublishedYear ?? existing.PublishedYear;

        var book = existing.Update(title, author, isbn, publishedYear);
        if (!string.Equals(book.ISBN, existing.ISBN, StringComparison.OrdinalIgnoreCase)
            && await bookRepository.ExistsByIsbnAsync(book.ISBN, cancellationToken))
        {
            throw new ValidationException($"A book with ISBN '{book.ISBN}' already exists.");
        }

        var updated = await bookRepository.UpdateAsync(book, cancellationToken);
        logger.LogInformation("Updated book {BookId}", id);
        return ToDto(updated);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        _ = await bookRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Book '{id}' was not found.");
        await bookRepository.DeleteAsync(id, cancellationToken);
        logger.LogInformation("Deleted book {BookId}", id);
    }

    private static BookResponseDto ToDto(Book book) =>
        new(book.Id, book.Title, book.Author, book.ISBN, book.PublishedYear, book.CreatedAt, book.UpdatedAt);
}
