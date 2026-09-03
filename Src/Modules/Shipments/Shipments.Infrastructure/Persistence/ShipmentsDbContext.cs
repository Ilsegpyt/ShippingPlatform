using BuildingBlocks.Application;
using Microsoft.EntityFrameworkCore;
using Shipments.Application.Abstractions;
using Shipments.Domain.Declarations;
using Shipments.Domain.Shipments;

namespace Shipments.Infrastructure.Persistence;

public sealed class ShipmentsDbContext : DbContext, IShipmentsUnitOfWork
{
    public ShipmentsDbContext(
        DbContextOptions<ShipmentsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Shipment> Shipments => Set<Shipment>();

    public DbSet<DeclarationFile> DeclarationFiles =>
        Set<DeclarationFile>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ShipmentsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}