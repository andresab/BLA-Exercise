using BookCatalog.Domain.Entities;
using BookCatalog.Domain.Exceptions;
using FluentAssertions;

namespace BookCatalog.Tests.Unit.Domain;

public sealed class BookTests
{
    [Fact]
    public void Create_ThrowsValidationException_WhenTitleIsEmpty()
    {
        var act = () => Book.Create("", "Author", "123456789X", 2000);

        act.Should().Throw<ValidationException>().WithMessage("*Title*");
    }

    [Fact]
    public void Create_ThrowsValidationException_WhenIsbnFormatIsInvalid()
    {
        var act = () => Book.Create("Title", "Author", "bad", 2000);

        act.Should().Throw<ValidationException>().WithMessage("*ISBN*");
    }

    [Fact]
    public void Create_ReturnsTrimmedValidBook()
    {
        var book = Book.Create(" Title ", " Author ", " 123456789X ", 2000);

        book.Title.Should().Be("Title");
        book.Author.Should().Be("Author");
        book.ISBN.Should().Be("123456789X");
        book.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        book.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Update_ReturnsValidatedUpdatedBook()
    {
        var book = Book.Create("Title", "Author", "123456789X", 2000);

        var updated = book.Update(" New Title ", "New Author", "9780441172719", 1965);

        updated.Id.Should().Be(book.Id);
        updated.Title.Should().Be("New Title");
        updated.ISBN.Should().Be("9780441172719");
        updated.CreatedAt.Should().Be(book.CreatedAt);
        updated.UpdatedAt.Should().BeAfter(book.UpdatedAt);
    }
}
