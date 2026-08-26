using BuildingBlocks.Application;
using Customers.Application; 
using Customers.Application.Abstractions;
using Customers.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Customers.Infrastructure;

public static class CustomersModuleServiceCollectionExtensions
{
    public static IServiceCollection AddCustomersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CustomersDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("CustomersDb")));

        services.AddScoped<ICustomersUnitOfWork>(
        sp => sp.GetRequiredService<CustomersDbContext>());


        //services.AddScoped<IUnitOfWork>(sp =>
        //sp.GetRequiredService<CustomersDbContext>());

        services.AddScoped<ICustomerRepository, CustomerRepository>();

        services.AddCustomersApplication(); 

        return services;
    }
}