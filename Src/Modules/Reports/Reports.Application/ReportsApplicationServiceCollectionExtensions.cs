using BuildingBlocks.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Reports.Application.Reports.UploadReport;

namespace Reports.Application;

public static class ReportsApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddReportsApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(UploadReportCommandHandler).Assembly);

            cfg.AddOpenBehavior(
                typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(
            typeof(ReportsApplicationServiceCollectionExtensions).Assembly);

        return services;
    }
}