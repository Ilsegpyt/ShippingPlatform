using Microsoft.EntityFrameworkCore;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;
using Operations.Domain.Enums;

namespace Operations.Infrastructure.Persistence.Repositories;

public sealed class FclRepository(OperationsDbContext dbContext)
    : IFclRepository
{
    public Task<OperationContainer?> GetImportContainerForApprovalAsync(
        Guid containerId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.OperationContainers
            .Include(c => c.Operation)
                .ThenInclude(o => o.ImportDetails)
            .Include(c => c.Operation)
                .ThenInclude(o => o.ShippingLine)
            .FirstOrDefaultAsync(
                c => c.Id == containerId
                     && c.Operation.OperationType == OperationType.Import
                     && c.Operation.Status == OperationStatus.Open,
                cancellationToken);
    }

    public Task<bool> IsImportContainerApprovedAsync(
        Guid importContainerId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.FclApprovals
            .AnyAsync(
                x => x.ImportContainerId == importContainerId,
                cancellationToken);
    }

    public async Task<HashSet<Guid>> GetApprovedImportContainerIdsAsync(
        CancellationToken cancellationToken = default)
    {
        var ids = await dbContext.FclApprovals
            .Select(x => x.ImportContainerId)
            .ToListAsync(cancellationToken);

        return ids.ToHashSet();
    }

    public Task<bool> ApprovalKeyExistsAsync(
        string approvalKey,
        CancellationToken cancellationToken = default)
    {
        return dbContext.FclApprovals
            .AnyAsync(
                x => x.ApprovalKey == approvalKey,
                cancellationToken);
    }

    public async Task AddApprovalAsync(
        FclApproval approval,
        CancellationToken cancellationToken = default)
    {
        await dbContext.FclApprovals.AddAsync(
            approval,
            cancellationToken);
    }

    public Task<OperationContainer?> GetExportContainerForMatchingAsync(
        Guid exportContainerId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.OperationContainers
            .Include(x => x.Operation)
                .ThenInclude(x => x.ExportDetails)
                    .ThenInclude(x => x!.POL)
            .Include(x => x.Operation)
                .ThenInclude(x => x.ShippingLine)
            .FirstOrDefaultAsync(
                x => x.Id == exportContainerId
                     && x.Operation.OperationType == OperationType.Export
                     && x.Operation.Status == OperationStatus.Open
                     && x.ContainerNumber == null,
                cancellationToken);
    }

    public async Task<IReadOnlyList<OperationContainer>>
        GetAllExportContainersForMatchingAsync(
            CancellationToken cancellationToken = default)
    {
        return await dbContext.OperationContainers
            .Where(c =>
                c.ContainerNumber == null
                && c.RequiredLoadingDate.HasValue
                && c.RequiredLoadingTime.HasValue
                && c.ContainerType.HasValue
                && c.Operation.OperationType == OperationType.Export
                && c.Operation.Status == OperationStatus.Open
                && c.Operation.ExportDetails != null)
            .Include(c => c.Operation)
                .ThenInclude(o => o.ExportDetails)
                    .ThenInclude(d => d!.POL)
            .Include(c => c.Operation)
                .ThenInclude(o => o.ShippingLine)
            .OrderBy(c => c.Operation.CreatedAtUtc)
            .ThenBy(c => c.Ordinal)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Operation>> GetAllImportCandidatesAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Operations
            .Where(o =>
                o.OperationType == OperationType.Import
                && o.Status == OperationStatus.Open
                && o.ReopenedAtUtc == null
                && o.ImportDetails != null)
            .Include(o => o.ShippingLine)
            .Include(o => o.ImportDetails)
                .ThenInclude(d => d!.POD)
            .Include(o => o.Containers.Where(c => !dbContext.FclApprovals.Any(a => a.ImportContainerId == c.Id)))
            .OrderBy(o => o.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Operation>> GetImportCandidatesAsync(
        Guid clientId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Operations
            .Where(x =>
                x.ClientId == clientId
                && x.OperationType == OperationType.Import
                && x.Status == OperationStatus.Open
                && x.ReopenedAtUtc == null)
            .Include(x => x.ImportDetails)
            .Include(x => x.Containers)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FclApproval>> GetAllApprovalsAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.FclApprovals
            .AsNoTracking()
            .OrderByDescending(x => x.ApprovedAtUtc)
            .ToListAsync(cancellationToken);
    }
}
