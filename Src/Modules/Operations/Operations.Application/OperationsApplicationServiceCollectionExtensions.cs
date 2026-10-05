using BuildingBlocks.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Operations.Application.Abstractions;

namespace Operations.Application;

public static class OperationsApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddOperationsApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(IOperationsUnitOfWork).Assembly);

            cfg.AddOpenBehavior(
                typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(
            typeof(IOperationsUnitOfWork).Assembly);

        return services;
    }
}
