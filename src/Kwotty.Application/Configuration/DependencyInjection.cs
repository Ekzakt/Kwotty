using Kwotty.Application.Contracts;
using Kwotty.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace Kwotty.Application.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IQuoteService, QuoteService> ();

        return services;
    }
}
