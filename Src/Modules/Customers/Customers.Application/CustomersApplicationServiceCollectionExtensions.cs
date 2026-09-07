using BuildingBlocks.Application.Behaviors;
using Customers.Application.Commands.RegisterCustomer;
using Customers.Application.Services;
using Customers.Contracts;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Customers.Application;

public static class CustomersApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddCustomersApplication(
        this IServiceCollection services)
    {

        services.AddScoped<CustomerQueries>();

        services.AddScoped<ICustomerQueries>(sp =>
            sp.GetRequiredService<CustomerQueries>());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(RegisterCustomerCommandHandler).Assembly);

            cfg.AddOpenBehavior(
            typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(
           typeof(CustomersApplicationServiceCollectionExtensions).Assembly);

        return services;
    }
}