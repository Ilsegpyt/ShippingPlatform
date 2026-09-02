using Customers.Application.Abstractions;
using Customers.Domain.SearchHistory;
using Microsoft.EntityFrameworkCore;

namespace Customers.Infrastructure.Persistence.Repositories;

public sealed class SearchHistoryRepository(
    CustomersDbContext dbContext)
    : ISearchHistoryRepository
{
    public async Task AddAsync(
        SearchHistory searchHistory,
        CancellationToken ct)
    {
        await dbContext.SearchHistories.AddAsync(
            searchHistory,
            ct);
    }

    public async Task<IReadOnlyList<SearchHistory>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken ct)
    {
        return await dbContext.SearchHistories
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.SearchedOnUtc)
            .ToListAsync(ct);
    }
}