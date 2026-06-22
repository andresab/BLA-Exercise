namespace BookCatalog.Application.Books;

public interface IBookService
{
    Task<IEnumerable<BookResponseDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<BookResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<BookResponseDto> CreateAsync(CreateBookDto dto, CancellationToken cancellationToken);
    Task<BookResponseDto> UpdateAsync(Guid id, UpdateBookDto dto, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
