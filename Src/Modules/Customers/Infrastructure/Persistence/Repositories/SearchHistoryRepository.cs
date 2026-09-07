using Customers.Application.Abstractions;
using Customers.Domain.SearchHistory;
using Microsoft.EntityFrameworkCore;

namespace Customers.Infrastructure.Persistence.Repositories;

public sealed class SearchHistoryRepository(CustomersDbContext dbContext)
    : ISearchHistoryRepository
{
    public void Add(SearchHistory searchHistory)
    {
        dbContext.SearchHistories.Add(searchHistory);
    }

    public async Task<IReadOnlyList<SearchHistory>> GetAllAsync(int skip, int take, CancellationToken ct)
    {
        return await dbContext.SearchHistories
            .AsNoTracking()
            .OrderByDescending(x => x.SearchedOnUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct)
    {
        return await dbContext.SearchHistories.CountAsync(ct);
    }
}