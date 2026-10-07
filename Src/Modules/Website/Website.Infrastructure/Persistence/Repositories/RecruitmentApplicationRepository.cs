using Microsoft.EntityFrameworkCore;
using Website.Application.Abstractions.Repositories;
using Website.Domain.Entities;

namespace Website.Infrastructure.Persistence.Repositories;

public sealed class RecruitmentApplicationRepository(
    WebsiteDbContext dbContext)
    : IRecruitmentApplicationRepository
{
    public void Add(RecruitmentApplication application)
    {
        dbContext.RecruitmentApplications.Add(application);
    }

    public async Task<(IReadOnlyList<RecruitmentApplication> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct)
    {
        var query = dbContext.RecruitmentApplications
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<RecruitmentApplication?> GetByIdAsync(
        int id,
        CancellationToken ct)
    {
        return await dbContext.RecruitmentApplications
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}