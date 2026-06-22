using TaskManagement.Application.Common;
using TaskManagement.Application.Users;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Tasks;

public sealed class TaskService(
    ITaskRepository taskRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : ITaskService
{
    public async Task<TaskResponse> CreateAsync(Guid currentUserId, CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureUserExistsAsync(currentUserId, cancellationToken);

        var task = TaskItem.Create(
            request.Title,
            request.Description,
            request.Status,
            request.DueDate,
            currentUserId);

        await taskRepository.AddAsync(task, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(currentUserId, task.Id, cancellationToken);
    }

    public async Task<TaskResponse> GetByIdAsync(Guid currentUserId, Guid id, CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetByIdForUserAsync(id, currentUserId, cancellationToken);

        return task is null ? throw new NotFoundException("Task", id) : ToResponse(task);
    }

    public async Task<IReadOnlyCollection<TaskResponse>> GetAllAsync(Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var tasks = await taskRepository.ListByUserAsync(currentUserId, cancellationToken);
        return tasks.Select(ToResponse).ToList();
    }

    public async Task<TaskResponse> UpdateAsync(Guid currentUserId, Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetByIdForUserAsync(id, currentUserId, cancellationToken);
        if (task is null)
        {
            throw new NotFoundException("Task", id);
        }

        task.Update(
            request.Title,
            request.Description,
            request.Status,
            request.DueDate,
            currentUserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(currentUserId, id, cancellationToken);
    }

    public async Task DeleteAsync(Guid currentUserId, Guid id, CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetByIdForUserAsync(id, currentUserId, cancellationToken);
        if (task is null)
        {
            throw new NotFoundException("Task", id);
        }

        taskRepository.Delete(task);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<TaskResponse>> GetByUserAsync(Guid currentUserId, Guid userId, CancellationToken cancellationToken = default)
    {
        if (currentUserId != userId)
        {
            throw new NotFoundException("User", userId);
        }

        await EnsureUserExistsAsync(userId, cancellationToken);

        var tasks = await taskRepository.ListByUserAsync(userId, cancellationToken);
        return tasks.Select(ToResponse).ToList();
    }

    private async Task EnsureUserExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User", userId);
        }
    }

    private static TaskResponse ToResponse(TaskItem task)
    {
        return new TaskResponse(
            task.Id,
            task.Title,
            task.Description,
            task.Status,
            task.DueDate,
            task.UserId,
            new UserSummaryResponse(task.User.Id, task.User.Name, task.User.Email));
    }
}
