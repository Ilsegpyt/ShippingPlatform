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
        string? search = null,
        CancellationToken ct = default)
    {
        var query = dbContext.RecruitmentApplications
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(x =>
                EF.Functions.Like(x.Department, $"%{s}%") ||
                EF.Functions.Like(x.CvFileName, $"%{s}%") ||
                EF.Functions.Like(x.Message, $"%{s}%"));
        }

        query = query.OrderByDescending(x => x.CreatedAtUtc);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<RecruitmentApplication?> GetByIdAsync(
        int id,
        CancellationToken ct = default)
    {
        return await dbContext.RecruitmentApplications
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}
