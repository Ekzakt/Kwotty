using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kwotty.Data.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<KwottyDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("KwottyConnectionString"))
        );

        return services;
    }
}
