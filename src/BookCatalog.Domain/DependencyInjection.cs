using Microsoft.Extensions.DependencyInjection;

namespace BookCatalog.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services) => services;
}
