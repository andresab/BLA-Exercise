using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Tasks;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Repositories;

public sealed class TaskRepository(ApplicationDbContext dbContext) : ITaskRepository
{
    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        await dbContext.Tasks.AddAsync(task, cancellationToken);
    }

    public Task<TaskItem?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return QueryTasks()
            .SingleOrDefaultAsync(task => task.Id == id && task.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<TaskItem>> ListByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await QueryTasks()
            .AsNoTracking()
            .Where(task => task.UserId == userId)
            .OrderBy(task => task.DueDate)
            .ThenBy(task => task.Title)
            .ToListAsync(cancellationToken);
    }

    public void Delete(TaskItem task)
    {
        dbContext.Tasks.Remove(task);
    }

    private IQueryable<TaskItem> QueryTasks()
    {
        return dbContext.Tasks.Include(task => task.User);
    }
}
