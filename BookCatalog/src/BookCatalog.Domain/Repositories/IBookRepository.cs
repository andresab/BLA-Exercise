using BookCatalog.Domain.Entities;

namespace BookCatalog.Domain.Repositories;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken);
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Book> CreateAsync(Book book, CancellationToken cancellationToken);
    Task<Book> UpdateAsync(Book book, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByIsbnAsync(string isbn, CancellationToken cancellationToken);
}
