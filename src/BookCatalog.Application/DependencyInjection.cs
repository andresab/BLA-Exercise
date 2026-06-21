using BookCatalog.Application.Books;
using BookCatalog.Application.Users;
using Microsoft.Extensions.DependencyInjection;

namespace BookCatalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}
