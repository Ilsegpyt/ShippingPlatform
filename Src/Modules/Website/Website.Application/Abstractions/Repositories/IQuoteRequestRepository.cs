using Website.Domain.Entities;

namespace Website.Application.Abstractions.Repositories;

public interface IQuoteRequestRepository
{
    void Add(QuoteRequest request);

    Task<(IReadOnlyList<QuoteRequest> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        CancellationToken ct = default);

    Task<QuoteRequest?> GetByIdAsync(
        int id,
        CancellationToken ct = default);
}
