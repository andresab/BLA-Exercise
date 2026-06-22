using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;
using DomainTaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("tasks");

        builder.HasKey(task => task.Id);

        builder.Property(task => task.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(task => task.Title)
            .HasColumnName("title")
            .HasMaxLength(TaskItem.TitleMaxLength)
            .IsRequired();

        builder.Property(task => task.Description)
            .HasColumnName("description")
            .HasMaxLength(TaskItem.DescriptionMaxLength);

        builder.Property(task => task.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(task => task.DueDate)
            .HasColumnName("due_date")
            .IsRequired();

        builder.Property(task => task.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.HasIndex(task => task.UserId);
        builder.HasIndex(task => task.DueDate);

        builder.HasData(
            new
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Title = "Prepare sprint planning",
                Description = "Review backlog and define priorities.",
                Status = DomainTaskStatus.Pending,
                DueDate = new DateTime(2026, 7, 1, 14, 0, 0, DateTimeKind.Utc),
                UserId = Guid.Parse("11111111-1111-1111-1111-111111111111")
            },
            new
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Title = "Publish release notes",
                Description = "Send the final version to stakeholders.",
                Status = DomainTaskStatus.InProgress,
                DueDate = new DateTime(2026, 7, 3, 12, 0, 0, DateTimeKind.Utc),
                UserId = Guid.Parse("11111111-1111-1111-1111-111111111111")
            },
            new
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Title = "Review production checklist",
                Description = "Confirm operational readiness items.",
                Status = DomainTaskStatus.Pending,
                DueDate = new DateTime(2026, 7, 5, 16, 0, 0, DateTimeKind.Utc),
                UserId = Guid.Parse("22222222-2222-2222-2222-222222222222")
            });
    }
}
