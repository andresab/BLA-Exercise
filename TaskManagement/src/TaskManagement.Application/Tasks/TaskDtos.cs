using System.ComponentModel.DataAnnotations;
using DomainTaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Tasks;

public sealed record UserSummaryResponse(
    Guid Id,
    string Name,
    string Email);

public sealed record TaskResponse(
    Guid Id,
    string Title,
    string? Description,
    DomainTaskStatus Status,
    DateTime DueDate,
    Guid UserId,
    UserSummaryResponse User);

public sealed record CreateTaskRequest(
    [Required]
    [MaxLength(200)]
    string Title,

    [MaxLength(2000)]
    string? Description,

    DomainTaskStatus Status,

    DateTime DueDate,

    Guid UserId);

public sealed record UpdateTaskRequest(
    [Required]
    [MaxLength(200)]
    string Title,

    [MaxLength(2000)]
    string? Description,

    DomainTaskStatus Status,

    DateTime DueDate,

    Guid UserId);
