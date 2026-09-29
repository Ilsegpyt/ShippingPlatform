using ContentEntity = Content.Domain.Entities.Content;
using Content.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Content.Infrastructure.Persistence.Repositories;

public sealed class ContentRepository : IContentRepository
{
    private readonly ContentDbContext _dbContext;

    public ContentRepository(ContentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(ContentEntity content)
    {
        _dbContext.Contents.Add(content);
    }

    public async Task<ContentEntity?> GetByIdAsync(
        Guid id,
        CancellationToken ct)
    {
        return await _dbContext.Contents
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<ContentEntity>> ListAsync(
        CancellationToken ct)
    {
        return await _dbContext.Contents
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ContentEntity>> GetChildrenAsync(
        Guid? parentId,
        CancellationToken ct)
    {
        return await _dbContext.Contents
            .AsNoTracking()
            .Where(x => x.ParentId == parentId)
            .ToListAsync(ct);
    }
    public void Delete(ContentEntity content)
    {
        _dbContext.Contents.Remove(content);
    }
}