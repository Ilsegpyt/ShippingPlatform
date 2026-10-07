using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Website.Application.Abstractions;

namespace Website.Application;

public static class WebsiteApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddWebsiteApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(IWebsiteUnitOfWork).Assembly);
        });

        services.AddValidatorsFromAssembly(
            typeof(IWebsiteUnitOfWork).Assembly);

        return services;
    }
}