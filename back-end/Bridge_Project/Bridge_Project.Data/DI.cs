using Bridge_Project.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bridge_Project.Data;

public static class DI
{
    public static IServiceCollection AddPersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<BridgeContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(BridgeContext).Assembly.FullName)), ServiceLifetime.Singleton);
        services.Add(new ServiceDescriptor(typeof(IBridgeContext), provider => provider.GetService<BridgeContext>(), ServiceLifetime.Singleton));

        return services;
    }
}
