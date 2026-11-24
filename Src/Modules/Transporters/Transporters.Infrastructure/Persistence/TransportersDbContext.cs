using Microsoft.EntityFrameworkCore;
using Transporters.Application.Abstractions;
using Transporters.Domain.Entities;

namespace Transporters.Infrastructure.Persistence;

public class TransportersDbContext : DbContext, ITransportersUnitOfWork
{
    public TransportersDbContext(DbContextOptions<TransportersDbContext> options)
        : base(options)
    {
    }

    public DbSet<Transporter> Transporters => Set<Transporter>();
    public DbSet<TransporterPortCoverage> PortCoverages => Set<TransporterPortCoverage>();
    public DbSet<TransporterLaneRate> LaneRates => Set<TransporterLaneRate>();
    public DbSet<TransporterClientException> ClientExceptions => Set<TransporterClientException>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("transporters");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(TransportersDbContext).Assembly);
    }
}
