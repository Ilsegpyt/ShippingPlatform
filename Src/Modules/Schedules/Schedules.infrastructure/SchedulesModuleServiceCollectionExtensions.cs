using BuildingBlocks.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Schedules.Application.Abstractions;
using Schedules.Application.Schedules.ImportSchedules;
using Schedules.Infrastructure.Excel;
using Schedules.Infrastructure.Persistence;
using Schedules.Infrastructure.Persistence.Repositories;

namespace Schedules.Infrastructure;

public static class SchedulesModuleServiceCollectionExtensions
{
    public static IServiceCollection AddSchedulesInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<SchedulesDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("SchedulesConnection")));

        services.AddScoped<ISchedulesUnitOfWork>(sp =>
            sp.GetRequiredService<SchedulesDbContext>());

        services.AddScoped<IUnitOfWork>(sp =>
            sp.GetRequiredService<ISchedulesUnitOfWork>());

        services.AddScoped<IScheduleRepository, ScheduleRepository>();

        services.AddScoped<IScheduleExcelReader, ScheduleExcelReader>();

        services.AddScoped<ImportScheduleRowParser>();

        services.AddScoped<ImportScheduleRowValidator>();
        services.AddScoped<IScheduleExcelWriter, ScheduleExcelWriter>();

        return services;
    }
}