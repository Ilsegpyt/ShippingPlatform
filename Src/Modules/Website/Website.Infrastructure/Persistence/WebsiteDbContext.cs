using Microsoft.EntityFrameworkCore;
using Website.Application.Abstractions;
using Website.Domain.Entities;

namespace Website.Infrastructure.Persistence;

public class WebsiteDbContext : DbContext, IWebsiteUnitOfWork
{
    public WebsiteDbContext(DbContextOptions<WebsiteDbContext> options)
        : base(options)
    {
    }

    public DbSet<AgentApplication> AgentApplications => Set<AgentApplication>();
    public DbSet<RecruitmentApplication> RecruitmentApplications => Set<RecruitmentApplication>();
    public DbSet<ContactInquiry> ContactInquiries => Set<ContactInquiry>();
    public DbSet<QuoteRequest> QuoteRequests => Set<QuoteRequest>();
    public DbSet<Branch> Branches => Set<Branch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(WebsiteDbContext).Assembly);
    }
}