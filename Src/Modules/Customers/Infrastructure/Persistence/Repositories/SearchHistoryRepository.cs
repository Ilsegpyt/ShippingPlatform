using Customers.Application.Abstractions;
using Customers.Domain.SearchHistory;
using Microsoft.EntityFrameworkCore;

namespace Customers.Infrastructure.Persistence.Repositories;

public sealed class SearchHistoryRepository(CustomersDbContext dbContext)
    : ISearchHistoryRepository
{
    public  void Add(SearchHistory searchHistory)
    {
         dbContext.SearchHistories.Add(searchHistory);
    }

    public async Task<IReadOnlyList<SearchHistory>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct)
    {
        return await dbContext.SearchHistories
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.SearchedOnUtc)
            .ToListAsync(ct);
    }
}