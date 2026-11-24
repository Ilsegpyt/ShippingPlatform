using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Transporters.Application.Abstractions;
using Transporters.Application.Abstractions.Repositories;
using Transporters.Infrastructure.Persistence;
using Transporters.Infrastructure.Persistence.Repositories;

namespace Transporters.Infrastructure;

public static class TransportersInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddTransportersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<TransportersDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("TransportersDb")));

        services.AddScoped<ITransportersUnitOfWork>(
            sp => sp.GetRequiredService<TransportersDbContext>());

        services.AddScoped<
            ITransporterRepository,
            TransporterRepository>();

        return services;
    }
}
