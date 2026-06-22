using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Tasks;

public interface ITaskRepository
{
    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TaskItem>> ListByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    void Delete(TaskItem task);
}
