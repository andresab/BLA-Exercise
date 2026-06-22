using BookCatalog.Application.Books;
using BookCatalog.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalog.API.Controllers;

[ApiController]
[Route("api/books")]
public sealed class BooksController(IBookService bookService, ILogger<BooksController> logger) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<BookResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        logger.LogInformation("GET api/books");
        var books = await bookService.GetAllAsync(cancellationToken);
        return Ok(books);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<BookResponseDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("GET api/books/{BookId}", id);
        try
        {
            var book = await bookService.GetByIdAsync(id, cancellationToken);
            return Ok(book);
        }
        catch (NotFoundException exception)
        {
            logger.LogWarning(exception, "Book {BookId} not found", id);
            return NotFound(new { error = exception.Message });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<BookResponseDto>> Create(CreateBookDto dto, CancellationToken cancellationToken)
    {
        logger.LogInformation("POST api/books");
        var created = await bookService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<BookResponseDto>> Update(Guid id, UpdateBookDto dto, CancellationToken cancellationToken)
    {
        logger.LogInformation("PUT api/books/{BookId}", id);
        try
        {
            var updated = await bookService.UpdateAsync(id, dto, cancellationToken);
            return Ok(updated);
        }
        catch (NotFoundException exception)
        {
            logger.LogWarning(exception, "Book {BookId} not found", id);
            return NotFound(new { error = exception.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("DELETE api/books/{BookId}", id);
        try
        {
            await bookService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (NotFoundException exception)
        {
            logger.LogWarning(exception, "Book {BookId} not found", id);
            return NotFound(new { error = exception.Message });
        }
    }
}
