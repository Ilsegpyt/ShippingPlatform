using BuildingBlocks.Application.Behaviors;
using FluentValidation;
using Identity.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;


namespace Identity.Application;

public static class IdentityApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(IIdentityUserService).Assembly);

            cfg.AddOpenBehavior(
                typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(
            typeof(IIdentityUserService).Assembly);

        return services;
    }
}