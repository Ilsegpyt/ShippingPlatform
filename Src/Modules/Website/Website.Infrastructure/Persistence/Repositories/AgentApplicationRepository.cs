using Microsoft.EntityFrameworkCore;
using Website.Application.Abstractions.Repositories;
using Website.Domain.Entities;

namespace Website.Infrastructure.Persistence.Repositories;

public sealed class AgentApplicationRepository(
    WebsiteDbContext dbContext)
    : IAgentApplicationRepository
{
    public void Add(AgentApplication application)
    {
        dbContext.AgentApplications.Add(application);
    }

    public async Task<(IReadOnlyList<AgentApplication> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        CancellationToken ct = default)
    {
        var query = dbContext.AgentApplications
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(x =>
                EF.Functions.Like(x.FirstName, $"%{s}%") ||
                EF.Functions.Like(x.LastName, $"%{s}%") ||
                EF.Functions.Like(x.Email, $"%{s}%") ||
                EF.Functions.Like(x.Phone, $"%{s}%") ||
                EF.Functions.Like(x.Country, $"%{s}%") ||
                EF.Functions.Like(x.City, $"%{s}%") ||
                EF.Functions.Like(x.MainIndustry, $"%{s}%"));
        }

        query = query.OrderByDescending(x => x.CreatedAtUtc);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<AgentApplication?> GetByIdAsync(
        int id,
        CancellationToken ct = default)
    {
        return await dbContext.AgentApplications
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}
