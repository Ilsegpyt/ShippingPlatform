using CustomsClearance.Application.Abstractions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CustomsClearance.Application;

public static class CustomsClearanceApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddCustomsClearanceApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(ICustomsClearanceUnitOfWork).Assembly);
        });

        services.AddValidatorsFromAssembly(
            typeof(ICustomsClearanceUnitOfWork).Assembly);

        return services;
    }
}
