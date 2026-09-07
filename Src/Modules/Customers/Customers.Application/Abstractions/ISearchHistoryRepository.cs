using Customers.Domain.SearchHistory;

namespace Customers.Application.Abstractions;

public interface ISearchHistoryRepository
{
    void Add(SearchHistory searchHistory);

    Task<IReadOnlyList<SearchHistory>> GetAllAsync(int skip, int take, CancellationToken ct);

    Task<int> CountAsync(CancellationToken ct);
}