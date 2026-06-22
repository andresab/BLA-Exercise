using FluentValidation;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Tasks;

public sealed class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(TaskItem.TitleMaxLength);

        RuleFor(request => request.Description)
            .MaximumLength(TaskItem.DescriptionMaxLength);

        RuleFor(request => request.Status)
            .IsInEnum();

        RuleFor(request => request.DueDate)
            .Must(dueDate => dueDate >= DateTime.UtcNow)
            .WithMessage("DueDate cannot be in the past.");

        RuleFor(request => request.UserId)
            .NotEmpty();
    }
}

public sealed class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(TaskItem.TitleMaxLength);

        RuleFor(request => request.Description)
            .MaximumLength(TaskItem.DescriptionMaxLength);

        RuleFor(request => request.Status)
            .IsInEnum();

        RuleFor(request => request.UserId)
            .NotEmpty();
    }
}
