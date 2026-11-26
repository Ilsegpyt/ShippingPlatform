using CustomsClearance.Application.Abstractions;
using CustomsClearance.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomsClearance.Infrastructure.Persistence;

public class CustomsClearanceDbContext : DbContext, ICustomsClearanceUnitOfWork
{
    public CustomsClearanceDbContext(DbContextOptions<CustomsClearanceDbContext> options)
        : base(options)
    {
    }

    public DbSet<CustomsShipment> CustomsShipments => Set<CustomsShipment>();
    public DbSet<OceanImportDetails> OceanImportDetails => Set<OceanImportDetails>();
    public DbSet<OceanExportDetails> OceanExportDetails => Set<OceanExportDetails>();
    public DbSet<DomesticImportDetails> DomesticImportDetails => Set<DomesticImportDetails>();
    public DbSet<CustomsContainer> CustomsContainers => Set<CustomsContainer>();
    public DbSet<CustomsUpdateLog> CustomsUpdateLogs => Set<CustomsUpdateLog>();
    public DbSet<CustomsFinanceRequest> CustomsFinanceRequests => Set<CustomsFinanceRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("customs");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CustomsClearanceDbContext).Assembly);
    }
}
