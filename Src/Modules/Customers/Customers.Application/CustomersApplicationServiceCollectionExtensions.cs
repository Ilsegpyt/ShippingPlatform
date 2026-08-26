using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Application.Contracts;
using Customers.Application.Contracts;
using Customers.Application.Customers.RegisterCustomer;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Customers.Application;

public static class CustomersApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddCustomersApplication(
        this IServiceCollection services)
    {
        services.AddScoped<CustomerRegistrar>();
        services.AddScoped<ICustomerRegistrar>(sp =>
            sp.GetRequiredService<CustomerRegistrar>());

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