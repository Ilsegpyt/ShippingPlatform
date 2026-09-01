
using Microsoft.Extensions.DependencyInjection;

namespace Schedules.Application;

public static class SchedulesModuleServiceCollectionExtensions
{
    public static IServiceCollection AddSchedulesApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(
                typeof(SchedulesModuleServiceCollectionExtensions).Assembly));

        return services;
    }
}

