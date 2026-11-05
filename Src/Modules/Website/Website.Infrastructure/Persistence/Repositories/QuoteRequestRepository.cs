using Microsoft.EntityFrameworkCore;
using Website.Application.Abstractions.Repositories;
using Website.Domain.Entities;

namespace Website.Infrastructure.Persistence.Repositories;

public sealed class QuoteRequestRepository(
    WebsiteDbContext dbContext)
    : IQuoteRequestRepository
{
    public void Add(QuoteRequest request)
    {
        dbContext.QuoteRequests.Add(request);
    }

    public async Task<(IReadOnlyList<QuoteRequest> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        CancellationToken ct = default)
    {
        var query = dbContext.QuoteRequests
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(x =>
                EF.Functions.Like(x.FirstName, $"%{s}%") ||
                EF.Functions.Like(x.LastName, $"%{s}%") ||
                EF.Functions.Like(x.Company, $"%{s}%") ||
                EF.Functions.Like(x.Email, $"%{s}%") ||
                EF.Functions.Like(x.Phone, $"%{s}%"));
        }

        query = query.OrderByDescending(x => x.CreatedAtUtc);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<QuoteRequest?> GetByIdAsync(
        int id,
        CancellationToken ct = default)
    {
        return await dbContext.QuoteRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}
