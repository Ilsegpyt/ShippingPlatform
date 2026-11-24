using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Transporters.Application.Abstractions;

namespace Transporters.Application;

public static class TransportersApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddTransportersApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(ITransportersUnitOfWork).Assembly);
        });

        services.AddValidatorsFromAssembly(
            typeof(ITransportersUnitOfWork).Assembly);

        return services;
    }
}
