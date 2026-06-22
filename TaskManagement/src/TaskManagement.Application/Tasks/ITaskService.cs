namespace TaskManagement.Application.Tasks;

public interface ITaskService
{
    Task<TaskResponse> CreateAsync(Guid currentUserId, CreateTaskRequest request, CancellationToken cancellationToken = default);
    Task<TaskResponse> GetByIdAsync(Guid currentUserId, Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TaskResponse>> GetAllAsync(Guid currentUserId, CancellationToken cancellationToken = default);
    Task<TaskResponse> UpdateAsync(Guid currentUserId, Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid currentUserId, Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TaskResponse>> GetByUserAsync(Guid currentUserId, Guid userId, CancellationToken cancellationToken = default);
}
