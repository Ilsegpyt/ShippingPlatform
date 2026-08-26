using Customers.Application.Abstractions;
using Customers.Domain;
using Microsoft.EntityFrameworkCore;

namespace Customers.Infrastructure.Persistence;

public sealed class CustomerRepository(CustomersDbContext dbContext) : ICustomerRepository
{
    public async Task AddAsync(Customer customer, CancellationToken ct)
        => await dbContext.Customers.AddAsync(customer, ct);

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct)
        => await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id, ct); // الـ Global Filter شغال هنا تلقائي

    public async Task<IReadOnlyList<Customer>> ListAsync(CancellationToken ct)
        => await dbContext.Customers.AsNoTracking().ToListAsync(ct); // بردو تحت الـ Global Filter (بس Active)

    public async Task<IReadOnlyList<Customer>> ListIgnoringDeletedFilterAsync(bool deletedOnly, CancellationToken ct)
    {
        var query = dbContext.Customers.IgnoreQueryFilters().AsNoTracking().AsQueryable();

        if (deletedOnly)
            query = query.Where(c => c.IsDeleted);

        return await query.ToListAsync(ct);
    }
    public async Task<Customer?> GetByOwnerUserIdAsync(Guid userId, CancellationToken ct)
    => await dbContext.Customers.FirstOrDefaultAsync(c => c.OwnerUserId == userId, ct);
}