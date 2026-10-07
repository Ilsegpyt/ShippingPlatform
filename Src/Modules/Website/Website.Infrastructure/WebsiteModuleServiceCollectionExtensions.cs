using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Website.Application.Abstractions;
using Website.Application.Abstractions.Repositories;
using Website.Infrastructure.Persistence;
using Website.Infrastructure.Persistence.Repositories;

namespace Website.Infrastructure;

public static class WebsiteModuleServiceCollectionExtensions
{
    public static IServiceCollection AddWebsiteModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<WebsiteDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("WebsiteDb")));

        services.AddScoped<IWebsiteUnitOfWork>(
       sp => sp.GetRequiredService<WebsiteDbContext>());

        services.AddScoped<
    IRecruitmentApplicationRepository,
    RecruitmentApplicationRepository>();

        services.AddScoped<
    IAgentApplicationRepository,
    AgentApplicationRepository>();


        services.AddScoped<
    IContactInquiryRepository,
    ContactInquiryRepository>();

        services.AddScoped<
    IQuoteRequestRepository,
    QuoteRequestRepository>();


        services.AddScoped<
    IBranchRepository,
    BranchRepository>();

        return services;
    }
}