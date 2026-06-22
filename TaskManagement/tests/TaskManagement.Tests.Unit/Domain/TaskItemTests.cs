using FluentAssertions;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Exceptions;
using DomainTaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Unit.Domain;

public sealed class TaskItemTests
{
    [Fact]
    public void Create_WithValidValues_NormalizesTask()
    {
        var userId = Guid.NewGuid();

        var task = TaskItem.Create(
            "  Prepare release  ",
            "  Validate deployment checklist  ",
            DomainTaskStatus.Pending,
            new DateTime(2026, 7, 1, 12, 0, 0, DateTimeKind.Utc),
            userId);

        task.Id.Should().NotBeEmpty();
        task.Title.Should().Be("Prepare release");
        task.Description.Should().Be("Validate deployment checklist");
        task.Status.Should().Be(DomainTaskStatus.Pending);
        task.UserId.Should().Be(userId);
    }

    [Fact]
    public void Create_WithBlankTitle_ThrowsValidationException()
    {
        var act = () => TaskItem.Create(
            "   ",
            null,
            DomainTaskStatus.Pending,
            DateTime.UtcNow,
            Guid.NewGuid());

        act.Should().Throw<DomainValidationException>()
            .WithMessage("Task title is required.");
    }

    [Fact]
    public void Update_WithDescriptionOverLimit_ThrowsValidationException()
    {
        var task = TaskItem.Create(
            "Existing task",
            null,
            DomainTaskStatus.Pending,
            DateTime.UtcNow,
            Guid.NewGuid());

        var act = () => task.Update(
            "Existing task",
            new string('x', TaskItem.DescriptionMaxLength + 1),
            DomainTaskStatus.InProgress,
            DateTime.UtcNow,
            Guid.NewGuid());

        act.Should().Throw<DomainValidationException>()
            .WithMessage($"Task description must be {TaskItem.DescriptionMaxLength} characters or fewer.");
    }
}
