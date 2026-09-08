using BuildingBlocks.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Shipments.Application.Services;
using Shipments.Contracts;

namespace Shipments.Application;

public static class ShipmentsApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddShipmentsApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(ShipmentsApplicationServiceCollectionExtensions).Assembly);

            cfg.AddOpenBehavior(
                typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(
            typeof(ShipmentsApplicationServiceCollectionExtensions).Assembly);

        services.AddScoped<IShipmentQueryService, ShipmentQueryService>();


        return services;
    }
}