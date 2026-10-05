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

        return services;
    }
}