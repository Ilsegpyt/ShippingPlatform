using BuildingBlocks.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Schedules.Application.Abstractions;
using Schedules.Application.Schedules.ImportSchedules;
using Schedules.Contracts;
using Schedules.Infrastructure.Excel;
using Schedules.Infrastructure.Persistence;
using Schedules.Infrastructure.Persistence.Repositories;
using Schedules.Infrastructure.Services;

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
        services.AddScoped<IScheduleSearchService, ScheduleSearchService>();
        services.AddScoped<IScheduleQueryService, ScheduleSearchService>();


        return services;
    }
}