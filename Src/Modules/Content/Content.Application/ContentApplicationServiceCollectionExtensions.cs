using BuildingBlocks.Application.Behaviors;
using Content.Application.Content.CreateContent;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Content.Application;

public static class ContentApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddContentApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(CreateContentCommandHandler).Assembly);

            cfg.AddOpenBehavior(
                typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(
            typeof(ContentApplicationServiceCollectionExtensions).Assembly);

        return services;
    }
}