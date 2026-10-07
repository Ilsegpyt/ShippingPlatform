using Website.Domain.Entities;

namespace Website.Application.Abstractions.Repositories;

public interface IQuoteRequestRepository
{
    void Add(QuoteRequest request);

    Task<(IReadOnlyList<QuoteRequest> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct);

    Task<QuoteRequest?> GetByIdAsync(
        int id,
        CancellationToken ct);
}