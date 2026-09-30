using Content.Application;
using Content.Application.Abstractions;
using Content.Infrastructure.Persistence;
using Content.Infrastructure.Persistence.Repositories;
using Content.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Content.Infrastructure;

public static class ContentModuleServiceCollectionExtensions
{
    public static IServiceCollection AddContentModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ContentDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("ContentDb")));

        services.AddScoped<IContentUnitOfWork>(
            sp => sp.GetRequiredService<ContentDbContext>());

        services.AddScoped<IContentRepository, ContentRepository>();


        services.AddScoped<
            IContentFileStorage,
            LocalContentFileStorage>();

        services.AddContentApplication();

        return services;
    }
}