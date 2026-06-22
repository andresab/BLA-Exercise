using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using TaskManagement.Application.Auth;
using TaskManagement.Application.Tasks;

namespace TaskManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddValidatorsFromAssemblyContaining<CreateTaskRequestValidator>();
        return services;
    }
}
