using CustomsClearance.Application.Abstractions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

        // Alerts read "today" in Cairo time through TimeProvider (replaceable in tests).
        services.TryAddSingleton(TimeProvider.System);

        return services;
    }
}
