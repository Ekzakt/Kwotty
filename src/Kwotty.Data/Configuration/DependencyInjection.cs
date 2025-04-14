using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kwotty.Data.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(Constants.KwottyConnectionString);

        services.AddDbContextFactory<KwottyDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlServerOptions =>
            {
                sqlServerOptions.MigrationsHistoryTable(Constants.MigrationsHistoryTableName, Constants.KwottySchema);
                sqlServerOptions.EnableRetryOnFailure(3);
            });
        });

        return services;
    }
}
