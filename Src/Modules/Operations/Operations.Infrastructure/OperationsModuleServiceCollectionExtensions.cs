using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Operations.Application.Abstractions;
using Operations.Infrastructure.Persistence;
using Operations.Infrastructure.Persistence.Repositories;

namespace Operations.Infrastructure;

public static class OperationsModuleServiceCollectionExtensions
{
    public static IServiceCollection AddOperationsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<OperationsDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("OperationsDb")));

        services.AddScoped<IOperationsUnitOfWork>(
            sp => sp.GetRequiredService<OperationsDbContext>());

        services.AddScoped<IOperationRepository, OperationRepository>();
        services.AddScoped<IFclRepository, FclRepository>();
        services.AddScoped<IPortRepository, PortRepository>();
        services.AddScoped<IDestinationRepository, DestinationRepository>();
        services.AddScoped<IDriverRepository, DriverRepository>();
        services.AddScoped<IShippingLineRepository, ShippingLineRepository>();
        services.AddScoped<ITransporterRepository, TransporterRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();

        // FTL Airport
        services.AddScoped<IFtlAirportRepository, FtlAirportRepository>();
        services.AddScoped<IFtlTerminalRepository, FtlTerminalRepository>();
        services.AddScoped<IFtlTransporterRepository, FtlTransporterRepository>();
        services.AddScoped<IFtlDriverRepository, FtlDriverRepository>();
        services.AddScoped<IFtlVehicleRepository, FtlVehicleRepository>();
        services.AddScoped<IFtlOperationRepository, FtlOperationRepository>();

        return services;
    }
}
