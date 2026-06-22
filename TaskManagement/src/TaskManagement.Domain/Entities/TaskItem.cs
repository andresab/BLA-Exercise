using TaskManagement.Domain.Exceptions;
using DomainTaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Domain.Entities;

public sealed class TaskItem
{
    public const int TitleMaxLength = 200;
    public const int DescriptionMaxLength = 2000;

    private TaskItem()
    {
        Title = string.Empty;
    }

    private TaskItem(
        Guid id,
        string title,
        string? description,
        DomainTaskStatus status,
        DateTime dueDate,
        Guid userId)
    {
        Title = string.Empty;
        Id = id;
        Apply(title, description, status, dueDate, userId);
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public DomainTaskStatus Status { get; private set; }
    public DateTime DueDate { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public static TaskItem Create(
        string title,
        string? description,
        DomainTaskStatus status,
        DateTime dueDate,
        Guid userId)
    {
        return new TaskItem(Guid.NewGuid(), title, description, status, dueDate, userId);
    }

    public void Update(
        string title,
        string? description,
        DomainTaskStatus status,
        DateTime dueDate,
        Guid userId)
    {
        Apply(title, description, status, dueDate, userId);
    }

    private void Apply(
        string title,
        string? description,
        DomainTaskStatus status,
        DateTime dueDate,
        Guid userId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainValidationException("Task title is required.");
        }

        var normalizedTitle = title.Trim();
        if (normalizedTitle.Length > TitleMaxLength)
        {
            throw new DomainValidationException($"Task title must be {TitleMaxLength} characters or fewer.");
        }

        var normalizedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (normalizedDescription?.Length > DescriptionMaxLength)
        {
            throw new DomainValidationException($"Task description must be {DescriptionMaxLength} characters or fewer.");
        }

        if (!Enum.IsDefined(status))
        {
            throw new DomainValidationException("Task status is invalid.");
        }

        if (userId == Guid.Empty)
        {
            throw new DomainValidationException("Task user id is required.");
        }

        Title = normalizedTitle;
        Description = normalizedDescription;
        Status = status;
        DueDate = dueDate;
        UserId = userId;
    }
}
