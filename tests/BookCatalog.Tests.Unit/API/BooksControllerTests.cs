using BookCatalog.API.Controllers;
using BookCatalog.Application.Books;
using BookCatalog.Domain.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookCatalog.Tests.Unit.API;

public sealed class BooksControllerTests
{
    private readonly Mock<IBookService> _service = new();
    private readonly BooksController _controller;

    public BooksControllerTests()
    {
        _controller = new BooksController(_service.Object, Mock.Of<ILogger<BooksController>>());
    }

    [Fact]
    public async Task GetAll_Returns200_WithBookList()
    {
        _service.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([SampleBook()]);

        var result = await _controller.GetAll(CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeAssignableTo<IEnumerable<BookResponseDto>>();
    }

    [Fact]
    public async Task GetById_Returns404_WhenNotFound()
    {
        _service.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Book not found."));

        var result = await _controller.GetById(Guid.NewGuid(), CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Create_Returns201_WithLocationHeader()
    {
        var dto = SampleBook();
        _service.Setup(x => x.CreateAsync(It.IsAny<CreateBookDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _controller.Create(new CreateBookDto("Dune", "Frank Herbert", "9780441172719", 1965), CancellationToken.None);

        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.ActionName.Should().Be(nameof(BooksController.GetById));
        created.Value.Should().Be(dto);
    }

    [Fact]
    public async Task Update_Returns200_WithUpdatedBook()
    {
        var dto = SampleBook() with { Title = "Updated" };
        _service.Setup(x => x.UpdateAsync(dto.Id, It.IsAny<UpdateBookDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _controller.Update(dto.Id, new UpdateBookDto("Updated", null, null, null), CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(dto);
    }

    [Fact]
    public async Task Delete_Returns204_OnSuccess()
    {
        var result = await _controller.Delete(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
    }

    private static BookResponseDto SampleBook() =>
        new(Guid.NewGuid(), "Dune", "Frank Herbert", "9780441172719", 1965, DateTime.UtcNow, DateTime.UtcNow);
}
