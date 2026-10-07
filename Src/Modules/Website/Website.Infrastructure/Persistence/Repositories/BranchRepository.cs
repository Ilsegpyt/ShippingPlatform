using Microsoft.EntityFrameworkCore;
using Website.Application.Abstractions.Repositories;
using Website.Domain.Entities;

namespace Website.Infrastructure.Persistence.Repositories;

public sealed class BranchRepository(WebsiteDbContext dbContext)
    : IBranchRepository
{
    public void Add(Branch branch)
    {
        dbContext.Branches.Add(branch);
    }

    public async Task<IReadOnlyList<Branch>> GetAllAsync(
      CancellationToken ct)
    {
        return await dbContext.Branches
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }


    public async Task<Branch?> GetByIdAsync(
        int id,
        CancellationToken ct)
    {
        return await dbContext.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id && x.IsActive,
                ct);
    }

    public async Task<Branch?> GetForUpdateAsync(
        int id,
        CancellationToken ct)
    {
        return await dbContext.Branches
            .FirstOrDefaultAsync(
                x => x.Id == id,
                ct);
    }
}