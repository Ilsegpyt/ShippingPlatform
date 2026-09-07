using Customers.Application.Abstractions;
using Customers.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Customers.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository(CustomersDbContext dbContext) : ICustomerRepository
{
    public void Add(Customer customer)
        =>  dbContext.Customers.Add(customer);

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct)
        => await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id, ct); 

    public async Task<IReadOnlyList<Customer>> ListAsync(CancellationToken ct)
        => await dbContext.Customers.AsNoTracking().ToListAsync(ct); 

    public async Task<IReadOnlyList<Customer>> ListIgnoringDeletedFilterAsync(bool deletedOnly, CancellationToken ct)
    {
        var query = dbContext.Customers.IgnoreQueryFilters().AsNoTracking().AsQueryable();

        if (deletedOnly)
            query = query.Where(c => c.IsDeleted);

        return await query.ToListAsync(ct);
    }
    public async Task<Customer?> GetByUserIdAsync(Guid userId, CancellationToken ct)
    => await dbContext.Customers.FirstOrDefaultAsync(c => c.OwnerUserId == userId, ct);
}