using Microsoft.EntityFrameworkCore;
using Shipments.Application.Abstractions;
using Shipments.Domain.Declarations;

namespace Shipments.Infrastructure.Persistence.Repositories;

public sealed class DeclarationFileRepository(
    ShipmentsDbContext dbContext)
    : IDeclarationFileRepository
{
    public async Task AddAsync(
        DeclarationFile declarationFile,
        CancellationToken ct = default)
    {
        await dbContext.DeclarationFiles.AddAsync(
            declarationFile,
            ct);
    }

    public async Task<IReadOnlyList<DeclarationFile>> GetByIdsAsync(
        Guid shipmentId,
        IReadOnlyCollection<Guid> declarationFileIds,
        CancellationToken ct = default)
    {
        return await dbContext.DeclarationFiles
            .Where(x =>
                x.ShipmentId == shipmentId &&
                declarationFileIds.Contains(x.Id))
            .ToListAsync(ct);
    }

    public void RemoveRange(
        IReadOnlyCollection<DeclarationFile> declarationFiles)
    {
        dbContext.DeclarationFiles.RemoveRange(declarationFiles);
    }

    public async Task<IReadOnlyList<DeclarationFile>> GetByShipmentIdsAsync(
        IReadOnlyCollection<Guid> shipmentIds,
        CancellationToken ct = default)
    {
        return await dbContext.DeclarationFiles
            .Where(x => shipmentIds.Contains(x.ShipmentId))
            .ToListAsync(ct);
    }
}