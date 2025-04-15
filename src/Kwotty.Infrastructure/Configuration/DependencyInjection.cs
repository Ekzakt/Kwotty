using Kwotty.Application.Contracts;
using Kwotty.Data.Repos;
using Microsoft.Extensions.DependencyInjection;
using Models = Kwotty.Domain.Models;
using Entities = Kwotty.Data.Entities;

namespace Kwotty.Infrastructure.Configuration;

public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure-level services like repositories and mappers to the DI container.
    /// Uses reflection to automatically register IMapper implementations.
    /// </summary>
    /// <param name="services">The IServiceCollection.</param>
    /// <returns>The IServiceCollection for chaining.</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        var infrastructureAssembly = typeof(DependencyInjection).Assembly;

        var mapperTypes = infrastructureAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapper<,>)))
            .ToList();

        foreach (var mapperType in mapperTypes)
        {
            var mapperInterface = mapperType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapper<,>));

            if (mapperInterface != null)
            {
                services.AddScoped(mapperInterface, mapperType);
            }
        }

        services.AddScoped<IGenericRepo<Models.AccessToken, int>, GenericRepo<Models.AccessToken, Entities.AccessToken, int>>();
        services.AddScoped<IGenericRepo<Models.Author, Guid>, GenericRepo<Models.Author, Entities.Author, Guid>>();
        services.AddScoped<IGenericRepo<Models.Category, Guid>, GenericRepo<Models.Category, Entities.Category, Guid>>();
        services.AddScoped<IGenericRepo<Models.Medium, Guid>, GenericRepo<Models.Medium, Entities.Medium, Guid>>();
        services.AddScoped<IGenericRepo<Models.Quote, Guid>, GenericRepo<Models.Quote, Entities.Quote, Guid>>();
        services.AddScoped<IGenericRepo<Models.QuoteItem, Guid>, GenericRepo<Models.QuoteItem, Entities.QuoteItem, Guid>>();
        services.AddScoped<IGenericRepo<Models.Rating, Guid>, GenericRepo<Models.Rating, Entities.Rating, Guid>>(); // Note: ID type for Rating domain might differ if composite
        services.AddScoped<IGenericRepo<Models.User, Guid>, GenericRepo<Models.User, Entities.User, Guid>>();
        services.AddScoped<IGenericRepo<Models.UserSettings, Guid>, GenericRepo<Models.UserSettings, Entities.UserSettings, Guid>>();

        return services;
    }
}
