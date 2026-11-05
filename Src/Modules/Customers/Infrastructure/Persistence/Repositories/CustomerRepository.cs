using Customers.Application.Abstractions;
using Customers.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Customers.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository(CustomersDbContext dbContext) : ICustomerRepository
{
    public void Add(Customer customer)
        => dbContext.Customers.Add(customer);

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct)
        => await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Customer>> ListAsync(int skip, int take, string? search = null, CancellationToken ct = default)
    {
        var query = dbContext.Customers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(c =>
                EF.Functions.Like(c.CompanyName, $"%{s}%") ||
                EF.Functions.Like(c.OwnerName, $"%{s}%") ||
                EF.Functions.Like(c.OwnerEmail, $"%{s}%") ||
                EF.Functions.Like(c.OwnerPhone, $"%{s}%") ||
                EF.Functions.Like(c.Industry, $"%{s}%"));
        }

        return await query
            .OrderBy(x => x.CompanyName)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? search = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(search))
            return await dbContext.Customers.CountAsync(ct);

        var s = search.Trim();
        return await dbContext.Customers
            .Where(c =>
                EF.Functions.Like(c.CompanyName, $"%{s}%") ||
                EF.Functions.Like(c.OwnerName, $"%{s}%") ||
                EF.Functions.Like(c.OwnerEmail, $"%{s}%") ||
                EF.Functions.Like(c.OwnerPhone, $"%{s}%") ||
                EF.Functions.Like(c.Industry, $"%{s}%"))
            .CountAsync(ct);
    }

    public async Task<IReadOnlyList<Customer>> ListIgnoringDeletedFilterAsync(bool deletedOnly, int skip, int take, string? search = null, CancellationToken ct = default)
    {
        var query = dbContext.Customers
            .IgnoreQueryFilters()
            .AsNoTracking()
            .AsQueryable();

        if (deletedOnly)
            query = query.Where(c => c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(c =>
                EF.Functions.Like(c.CompanyName, $"%{s}%") ||
                EF.Functions.Like(c.OwnerName, $"%{s}%") ||
                EF.Functions.Like(c.OwnerEmail, $"%{s}%") ||
                EF.Functions.Like(c.OwnerPhone, $"%{s}%") ||
                EF.Functions.Like(c.Industry, $"%{s}%"));
        }

        return await query
            .OrderBy(x => x.CompanyName)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<int> CountIgnoringDeletedFilterAsync(bool deletedOnly, string? search = null, CancellationToken ct = default)
    {
        var query = dbContext.Customers
            .IgnoreQueryFilters()
            .AsQueryable();

        if (deletedOnly)
            query = query.Where(c => c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(c =>
                EF.Functions.Like(c.CompanyName, $"%{s}%") ||
                EF.Functions.Like(c.OwnerName, $"%{s}%") ||
                EF.Functions.Like(c.OwnerEmail, $"%{s}%") ||
                EF.Functions.Like(c.OwnerPhone, $"%{s}%") ||
                EF.Functions.Like(c.Industry, $"%{s}%"));
        }

        return await query.CountAsync(ct);
    }

    public async Task<Customer?> GetOwnerByCustomerIdAsync(Guid customerId, CancellationToken ct)
        => await dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == customerId, ct);

    public async Task<Customer?> GetByUserIdAsync(Guid userId, CancellationToken ct)
        => await dbContext.Customers.FirstOrDefaultAsync(c => c.OwnerUserId == userId, ct);

    public async Task<IReadOnlyList<Customer>> ListByIdsAsync(IReadOnlyCollection<Guid> customerIds, int skip, int take, string? search = null, CancellationToken ct = default)
    {
        if (customerIds.Count == 0)
            return [];

        var query = dbContext.Customers
            .AsNoTracking()
            .Where(c => customerIds.Contains(c.Id));

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(c =>
                EF.Functions.Like(c.CompanyName, $"%{s}%") ||
                EF.Functions.Like(c.OwnerName, $"%{s}%") ||
                EF.Functions.Like(c.OwnerEmail, $"%{s}%") ||
                EF.Functions.Like(c.OwnerPhone, $"%{s}%") ||
                EF.Functions.Like(c.Industry, $"%{s}%"));
        }

        return await query
            .OrderBy(c => c.CompanyName)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<int> CountByIdsAsync(IReadOnlyCollection<Guid> customerIds, string? search = null, CancellationToken ct = default)
    {
        if (customerIds.Count == 0)
            return 0;

        var query = dbContext.Customers
            .AsNoTracking()
            .Where(c => customerIds.Contains(c.Id));

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(c =>
                EF.Functions.Like(c.CompanyName, $"%{s}%") ||
                EF.Functions.Like(c.OwnerName, $"%{s}%") ||
                EF.Functions.Like(c.OwnerEmail, $"%{s}%") ||
                EF.Functions.Like(c.OwnerPhone, $"%{s}%") ||
                EF.Functions.Like(c.Industry, $"%{s}%"));
        }

        return await query.CountAsync(ct);
    }
}
