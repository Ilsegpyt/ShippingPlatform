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
        CancellationToken ct)
    {
        var query = dbContext.AgentApplications
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
    public async Task<AgentApplication?> GetByIdAsync(
    int id,
    CancellationToken ct)
    {
        return await dbContext.AgentApplications
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}