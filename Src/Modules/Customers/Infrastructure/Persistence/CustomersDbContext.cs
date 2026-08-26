using BuildingBlocks.Application.Events;
using BuildingBlocks.Infrastructure.Persistence;
using Customers.Application.Abstractions;
using Customers.Domain;
using Microsoft.EntityFrameworkCore;

namespace Customers.Infrastructure.Persistence;

public sealed class CustomersDbContext : DbContext, ICustomersUnitOfWork
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public CustomersDbContext(
        DbContextOptions<CustomersDbContext> options,
        IDomainEventDispatcher domainEventDispatcher)
        : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
    }

    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(
            typeof(CustomersDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken ct = default)
    {
        var domainEvents = this.GetDomainEvents();

        var result = await base.SaveChangesAsync(ct);

        this.ClearDomainEvents();

        await _domainEventDispatcher.DispatchAsync(
            domainEvents,
            ct);

        return result;
    }
}