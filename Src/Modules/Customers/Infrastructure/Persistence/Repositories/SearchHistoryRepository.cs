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
    public void RemoveRange(IReadOnlyCollection<SearchHistory> searchHistories)
    {
        dbContext.SearchHistories.RemoveRange(searchHistories);
    }
    public async Task<IReadOnlyList<SearchHistory>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct)
    {
        return await dbContext.SearchHistories
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(ct);
    }
    public async Task<int> CountTodayAsync(
    DateTime fromUtc,
    DateTime toUtc,
    CancellationToken ct)
    {
        return await dbContext.SearchHistories
            .CountAsync(
                x => x.SearchedOnUtc >= fromUtc &&
                     x.SearchedOnUtc < toUtc,
                ct);
    }

    public async Task<int> CountDistinctCustomersTodayAsync(
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken ct)
    {
        return await dbContext.SearchHistories
            .Where(
                x => x.SearchedOnUtc >= fromUtc &&
                     x.SearchedOnUtc < toUtc)
            .Select(x => x.CustomerId)
            .Distinct()
            .CountAsync(ct);
    }
}