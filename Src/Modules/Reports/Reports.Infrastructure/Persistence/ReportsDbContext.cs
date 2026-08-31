using Microsoft.EntityFrameworkCore;
using Reports.Domain.Report;
using System.Reflection.Emit;

namespace Reports.Infrastructure.Persistence;

public sealed class ReportsDbContext : DbContext
{
    public ReportsDbContext(
        DbContextOptions<ReportsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Report> Reports => Set<Report>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ReportsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}