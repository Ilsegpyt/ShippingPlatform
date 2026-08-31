
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reports.Application.Abstractions;
using Reports.Infrastructure.FileStorage;
using Reports.Infrastructure.Persistence;
using Reports.Infrastructure.Repositories;

namespace Reports.Infrastructure;

public static class ReportsModuleServiceCollectionExtensions
{
    public static IServiceCollection AddReportsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ReportsDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("ReportsDb")));

        services.AddScoped<IReportRepository, ReportRepository>();

        services.AddScoped<IReportsUnitOfWork, ReportsUnitOfWork>();

        services.AddScoped<IReportFileStorage>(sp =>
        {
            var path = configuration["ReportStorage:Path"]
                ?? Path.Combine("Storage", "Reports");

            return new LocalReportFileStorage(path);
        });

        return services;
    }
}
