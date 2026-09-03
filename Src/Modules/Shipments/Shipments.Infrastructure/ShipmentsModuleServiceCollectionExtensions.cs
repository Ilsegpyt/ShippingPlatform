using BuildingBlocks.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shipments.Application.Abstractions;
using Shipments.Infrastructure.FileStorage;
using Shipments.Infrastructure.Persistence;
using Shipments.Infrastructure.Persistence.Repositories;

namespace Shipments.Infrastructure;

public static class ShipmentsModuleServiceCollectionExtensions
{
    public static IServiceCollection AddShipmentsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ShipmentsDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("ShipmentsDb"));
        });
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ShipmentsDbContext>());


        var storagePath = Path.Combine(
            AppContext.BaseDirectory,
            "Storage",
            "Shipments");

        services.AddSingleton<IFileStorage>(
            new LocalFileStorage(storagePath));

        services.AddScoped<IShipmentRepository, ShipmentRepository>();
        services.AddScoped<IDeclarationFileRepository, DeclarationFileRepository>();

        services.AddScoped<IShipmentsUnitOfWork>(sp =>
            sp.GetRequiredService<ShipmentsDbContext>());

        return services;
    }
}
