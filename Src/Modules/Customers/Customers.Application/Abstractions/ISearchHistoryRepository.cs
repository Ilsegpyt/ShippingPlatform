using Customers.Domain.SearchHistory;

namespace Customers.Application.Abstractions;

public interface ISearchHistoryRepository
{
    Task AddAsync(
        SearchHistory searchHistory,
        CancellationToken ct);

    Task<IReadOnlyList<SearchHistory>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken ct);
}