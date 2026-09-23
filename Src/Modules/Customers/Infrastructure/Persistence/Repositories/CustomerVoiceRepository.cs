using Customers.Application.Abstractions;
using Customers.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Customers.Infrastructure.Persistence.Repositories;

public sealed class CustomerVoiceRepository(
    CustomersDbContext dbContext)
    : ICustomerVoiceRepository
{
    public async Task AddAsync(
        CustomerVoice customerVoice,
        CancellationToken ct = default)
    {
        await dbContext.CustomerVoices.AddAsync(
            customerVoice,
            ct);
    }

    public async Task<IReadOnlyList<CustomerVoice>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken ct = default)
    {
        return await dbContext.CustomerVoices
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CustomerVoice>> GetAllAsync(
        CancellationToken ct = default)
    {
        return await dbContext.CustomerVoices
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct);
    }

    public async Task<CustomerVoice?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await dbContext.CustomerVoices
            .FirstOrDefaultAsync(
                x => x.Id == id,
                ct);
    }
}