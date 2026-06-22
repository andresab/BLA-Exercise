using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Tasks;

public sealed class TaskService : ITaskService
{
    private readonly IApplicationDbContext _dbContext;

    public TaskService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TaskResponse> CreateAsync(Guid currentUserId, CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureUserExistsAsync(currentUserId, cancellationToken);

        var task = TaskItem.Create(
            request.Title,
            request.Description,
            request.Status,
            request.DueDate,
            currentUserId);

        _dbContext.Tasks.Add(task);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(currentUserId, task.Id, cancellationToken);
    }

    public async Task<TaskResponse> GetByIdAsync(Guid currentUserId, Guid id, CancellationToken cancellationToken = default)
    {
        var task = await QueryTasks()
            .AsNoTracking()
            .SingleOrDefaultAsync(task => task.Id == id && task.UserId == currentUserId, cancellationToken);

        return task is null ? throw new NotFoundException("Task", id) : ToResponse(task);
    }

    public async Task<IReadOnlyCollection<TaskResponse>> GetAllAsync(Guid currentUserId, CancellationToken cancellationToken = default)
    {
        return await QueryTasks()
            .AsNoTracking()
            .Where(task => task.UserId == currentUserId)
            .OrderBy(task => task.DueDate)
            .ThenBy(task => task.Title)
            .Select(task => ToResponse(task))
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskResponse> UpdateAsync(Guid currentUserId, Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = await _dbContext.Tasks.SingleOrDefaultAsync(
            task => task.Id == id && task.UserId == currentUserId,
            cancellationToken);

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

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(currentUserId, id, cancellationToken);
    }

    public async Task DeleteAsync(Guid currentUserId, Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _dbContext.Tasks.SingleOrDefaultAsync(
            task => task.Id == id && task.UserId == currentUserId,
            cancellationToken);

        if (task is null)
        {
            throw new NotFoundException("Task", id);
        }

        _dbContext.Tasks.Remove(task);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<TaskResponse>> GetByUserAsync(Guid currentUserId, Guid userId, CancellationToken cancellationToken = default)
    {
        if (currentUserId != userId)
        {
            throw new NotFoundException("User", userId);
        }

        await EnsureUserExistsAsync(userId, cancellationToken);

        return await QueryTasks()
            .AsNoTracking()
            .Where(task => task.UserId == userId)
            .OrderBy(task => task.DueDate)
            .ThenBy(task => task.Title)
            .Select(task => ToResponse(task))
            .ToListAsync(cancellationToken);
    }

    private IQueryable<TaskItem> QueryTasks()
    {
        return _dbContext.Tasks.Include(task => task.User);
    }

    private async Task EnsureUserExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Users.AnyAsync(user => user.Id == userId, cancellationToken);
        if (!exists)
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
