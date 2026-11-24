using Microsoft.EntityFrameworkCore;
using Transporters.Application.Abstractions.Repositories;
using Transporters.Domain.Entities;

namespace Transporters.Infrastructure.Persistence.Repositories;

public sealed class TransporterRepository(TransportersDbContext dbContext)
    : ITransporterRepository
{
    public void Add(Transporter transporter)
    {
        dbContext.Transporters.Add(transporter);
    }

    public void AddPortCoverage(TransporterPortCoverage portCoverage)
    {
        dbContext.PortCoverages.Add(portCoverage);
    }

    public void AddClientException(TransporterClientException clientException)
    {
        dbContext.ClientExceptions.Add(clientException);
    }

    public async Task<(IReadOnlyList<Transporter> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        CancellationToken ct = default)
    {
        var query = dbContext.Transporters
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(x =>
                EF.Functions.Like(x.Name, $"%{s}%") ||
                EF.Functions.Like(x.ContactPerson!, $"%{s}%") ||
                EF.Functions.Like(x.Phone!, $"%{s}%") ||
                EF.Functions.Like(x.Email!, $"%{s}%"));
        }

        query = query.OrderByDescending(x => x.CreatedAtUtc);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<Transporter?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await dbContext.Transporters
            .AsNoTracking()
            .Include(x => x.PortCoverages)
                .ThenInclude(x => x.LaneRates)
            .Include(x => x.ClientExceptions)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Transporter?> GetForUpdateAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await dbContext.Transporters
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await dbContext.Transporters
            .AnyAsync(x => x.Id == id, ct);
    }

    public async Task<TransporterPortCoverage?> GetPortCoverageForUpdateAsync(
        Guid transporterId,
        string portName,
        CancellationToken ct = default)
    {
        return await dbContext.PortCoverages
            .Include(x => x.LaneRates)
            .FirstOrDefaultAsync(
                x => x.TransporterId == transporterId && x.PortName == portName,
                ct);
    }

    public async Task<TransporterPortCoverage?> GetPortCoverageForUpdateAsync(
        Guid portCoverageId,
        CancellationToken ct = default)
    {
        return await dbContext.PortCoverages
            .FirstOrDefaultAsync(x => x.Id == portCoverageId, ct);
    }

    public async Task<TransporterPortCoverage?> GetPortCoverageByIdAsync(
        Guid transporterId,
        Guid portCoverageId,
        CancellationToken ct = default)
    {
        return await dbContext.PortCoverages
            .AsNoTracking()
            .Include(x => x.LaneRates)
            .Include(x => x.ClientExceptions)
            .AsSplitQuery()
            .FirstOrDefaultAsync(
                x => x.Id == portCoverageId && x.TransporterId == transporterId,
                ct);
    }
}
