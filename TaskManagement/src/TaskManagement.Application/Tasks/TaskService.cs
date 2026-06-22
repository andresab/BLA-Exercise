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

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureUserExistsAsync(request.UserId, cancellationToken);

        var task = TaskItem.Create(
            request.Title,
            request.Description,
            request.Status,
            request.DueDate,
            request.UserId);

        _dbContext.Tasks.Add(task);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(task.Id, cancellationToken);
    }

    public async Task<TaskResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await QueryTasks()
            .AsNoTracking()
            .SingleOrDefaultAsync(task => task.Id == id, cancellationToken);

        return task is null ? throw new NotFoundException("Task", id) : ToResponse(task);
    }

    public async Task<IReadOnlyCollection<TaskResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await QueryTasks()
            .AsNoTracking()
            .OrderBy(task => task.DueDate)
            .ThenBy(task => task.Title)
            .Select(task => ToResponse(task))
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskResponse> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = await _dbContext.Tasks.SingleOrDefaultAsync(task => task.Id == id, cancellationToken);
        if (task is null)
        {
            throw new NotFoundException("Task", id);
        }

        await EnsureUserExistsAsync(request.UserId, cancellationToken);

        task.Update(
            request.Title,
            request.Description,
            request.Status,
            request.DueDate,
            request.UserId);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _dbContext.Tasks.SingleOrDefaultAsync(task => task.Id == id, cancellationToken);
        if (task is null)
        {
            throw new NotFoundException("Task", id);
        }

        _dbContext.Tasks.Remove(task);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<TaskResponse>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
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
