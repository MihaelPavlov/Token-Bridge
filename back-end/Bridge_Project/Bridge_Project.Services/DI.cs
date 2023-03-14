using Bridge_Project.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bridge_Project.Services;

public static class DI
{
    public static IServiceCollection AddServiceLayer(this IServiceCollection services)
    {
        services.AddScoped<ISourceEventService, SourceEventService>();
        services.AddScoped<IDestinationEventService, DestinationEventService>();

        return services;
    }
}