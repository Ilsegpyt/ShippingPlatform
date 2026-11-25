using CustomsClearance.Application.Abstractions;
using CustomsClearance.Application.Abstractions.Repositories;
using CustomsClearance.Infrastructure.Persistence;
using CustomsClearance.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomsClearance.Infrastructure;

public static class CustomsClearanceModuleServiceCollectionExtensions
{
    public static IServiceCollection AddCustomsClearanceModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CustomsClearanceDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("CustomsClearanceDb")));

        services.AddScoped<ICustomsClearanceUnitOfWork>(
            sp => sp.GetRequiredService<CustomsClearanceDbContext>());

        services.AddScoped<
            ICustomsShipmentRepository,
            CustomsShipmentRepository>();

        return services;
    }
}
