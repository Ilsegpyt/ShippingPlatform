using BuildingBlocks.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Notifications.Application;

public static class NotificationsApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationsApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(NotificationsApplicationServiceCollectionExtensions).Assembly);

            cfg.AddOpenBehavior(
                typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(
            typeof(NotificationsApplicationServiceCollectionExtensions).Assembly);

        return services;
    }
}