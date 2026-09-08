using BuildingBlocks.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Tracking.Application;

public static class TrackingApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddTrackingApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(TrackingApplicationServiceCollectionExtensions).Assembly);

            cfg.AddOpenBehavior(
                typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(
            typeof(TrackingApplicationServiceCollectionExtensions).Assembly);

        return services;
    }
}