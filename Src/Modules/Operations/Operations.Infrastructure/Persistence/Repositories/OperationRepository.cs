using Microsoft.EntityFrameworkCore;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;
using Operations.Domain.Enums;

namespace Operations.Infrastructure.Persistence.Repositories;

public sealed class OperationRepository(
    OperationsDbContext dbContext)
    : IOperationRepository
{
    public async Task AddAsync(
        Operation operation,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Operations.AddAsync(operation, cancellationToken);
    }

    public Task<bool> DuplicateCheckKeyExistsAsync(
        Operations.Domain.Enums.OperationType operationType,
        string duplicateCheckKey,
        Guid excludeOperationId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Operations.AnyAsync(
            x => x.OperationType == operationType &&
                 x.DuplicateCheckKey == duplicateCheckKey &&
                 x.Id != excludeOperationId,
            cancellationToken);
    }

    public Task<Operation?> GetByIdForUpdateAsync(
        Guid operationId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Operations
            .Include(x => x.ImportDetails)
            .Include(x => x.ExportDetails)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Updates)
            .FirstOrDefaultAsync(x => x.Id == operationId, cancellationToken);
    }

    public Task<Operation?> GetByIdWithDetailsAsync(
        Guid operationId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Operations
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.ShippingLine)
            .Include(x => x.ImportDetails)
                .ThenInclude(x => x!.POD)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POL)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POW)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Transporter)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Driver)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Vehicle)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Updates)
            .FirstOrDefaultAsync(x => x.Id == operationId, cancellationToken);
    }


    public async Task<IReadOnlyList<Operation>> GetDeliveredWithDetailsAsync(
        OperationType? operationType,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Operations
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.Status == OperationStatus.Delivered);

        if (operationType.HasValue)
            query = query.Where(x => x.OperationType == operationType.Value);

        return await query
            .Include(x => x.ShippingLine)
            .Include(x => x.ImportDetails)
                .ThenInclude(x => x!.POD)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POL)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POW)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Transporter)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Driver)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Vehicle)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Updates)
            .OrderByDescending(x => x.DeliveredAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Operation>> GetAllWithDetailsAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Operations
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.ShippingLine)
            .Include(x => x.ImportDetails)
                .ThenInclude(x => x!.POD)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POL)
            .Include(x => x.ExportDetails)
                .ThenInclude(x => x!.POW)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Transporter)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Driver)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Vehicle)
            .Include(x => x.Containers)
                .ThenInclude(x => x.Updates)
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }
}
