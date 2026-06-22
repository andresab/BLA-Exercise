using BookCatalog.Application.Books;
using BookCatalog.Domain.Entities;
using BookCatalog.Domain.Exceptions;
using BookCatalog.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookCatalog.Tests.Unit.Application;

public sealed class BookServiceTests
{
    private readonly Mock<IBookRepository> _repository = new();
    private readonly BookService _service;

    public BookServiceTests()
    {
        _service = new BookService(_repository.Object, Mock.Of<ILogger<BookService>>());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDtos()
    {
        var book = SampleBook();
        _repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([book]);

        var result = await _service.GetAllAsync(CancellationToken.None);

        result.Should().ContainSingle().Which.Id.Should().Be(book.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenIdDoesNotExist()
    {
        _repository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Book?)null);

        var act = () => _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_ThrowsValidationException_WhenTitleIsEmpty()
    {
        var act = () => _service.CreateAsync(new CreateBookDto("", "Author", "123456789X", 2000), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>().WithMessage("*Title*");
    }

    [Fact]
    public async Task CreateAsync_ThrowsValidationException_WhenIsbnFormatIsInvalid()
    {
        var act = () => _service.CreateAsync(new CreateBookDto("Title", "Author", "bad", 2000), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>().WithMessage("*ISBN*");
    }

    [Fact]
    public async Task CreateAsync_ThrowsValidationException_WhenIsbnAlreadyExists()
    {
        _repository.Setup(x => x.ExistsByIsbnAsync("123456789X", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var act = () => _service.CreateAsync(new CreateBookDto("Title", "Author", "123456789X", 2000), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>().WithMessage("*already exists*");
    }

    [Fact]
    public async Task CreateAsync_ThrowsValidationException_WhenPublishedYearIsInFuture()
    {
        var act = () => _service.CreateAsync(new CreateBookDto("Title", "Author", "123456789X", DateTime.UtcNow.Year + 1), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>().WithMessage("*Published year*");
    }

    [Fact]
    public async Task CreateAsync_ReturnsDto_WhenBookIsValid()
    {
        _repository.Setup(x => x.ExistsByIsbnAsync("123456789X", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _repository.Setup(x => x.CreateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book book, CancellationToken _) => book);

        var result = await _service.CreateAsync(new CreateBookDto("Title", "Author", "123456789X", 2000), CancellationToken.None);

        result.Title.Should().Be("Title");
        result.ISBN.Should().Be("123456789X");
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFoundException_WhenIdDoesNotExist()
    {
        _repository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Book?)null);

        var act = () => _service.UpdateAsync(Guid.NewGuid(), new UpdateBookDto("New", null, null, null), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_ReturnsUpdatedDto_WhenValid()
    {
        var book = SampleBook();
        _repository.Setup(x => x.GetByIdAsync(book.Id, It.IsAny<CancellationToken>())).ReturnsAsync(book);
        _repository.Setup(x => x.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book updated, CancellationToken _) => updated);

        var result = await _service.UpdateAsync(book.Id, new UpdateBookDto("Updated", null, null, null), CancellationToken.None);

        result.Title.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteAsync_ThrowsNotFoundException_WhenIdDoesNotExist()
    {
        _repository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Book?)null);

        var act = () => _service.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_CallsRepository_WhenBookExists()
    {
        var book = SampleBook();
        _repository.Setup(x => x.GetByIdAsync(book.Id, It.IsAny<CancellationToken>())).ReturnsAsync(book);

        await _service.DeleteAsync(book.Id, CancellationToken.None);

        _repository.Verify(x => x.DeleteAsync(book.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Book SampleBook() =>
        new(Guid.NewGuid(), "Dune", "Frank Herbert", "9780441172719", 1965, DateTime.UtcNow, DateTime.UtcNow);
}
