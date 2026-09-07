using Customers.Domain.SearchHistory;

namespace Customers.Application.Abstractions;

public interface ISearchHistoryRepository
{
    void Add(SearchHistory searchHistory);
    Task<IReadOnlyList<SearchHistory>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct);
}