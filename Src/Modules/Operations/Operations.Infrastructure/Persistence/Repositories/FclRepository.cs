
using Microsoft.EntityFrameworkCore;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;
using Operations.Domain.Enums;

namespace Operations.Infrastructure.Persistence.Repositories;

public sealed class FclRepository(OperationsDbContext dbContext)
    : IFclRepository
{
    public Task<FclSuggestion?> GetSuggestionForApprovalAsync(
        Guid suggestionId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.FclSuggestions
            .Include(x => x.ImportOperation)
                .ThenInclude(x => x.ImportDetails)
            .Include(x => x.ImportContainer)
            .Include(x => x.ExportOperation)
                .ThenInclude(x => x!.ExportDetails)
            .Include(x => x.ExportContainer)
                .ThenInclude(x => x!.Operation)
                    .ThenInclude(x => x.ExportDetails)
            .FirstOrDefaultAsync(
                x => x.Id == suggestionId,
                cancellationToken);
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
            .FirstOrDefaultAsync(
                x => x.Id == exportContainerId
                     && x.Operation.OperationType == OperationType.Export
                     && x.Operation.Status == OperationStatus.Open,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Operation>> GetImportCandidatesAsync(
        Guid clientId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Operations
            .Where(x =>
                x.ClientId == clientId
                && x.OperationType == OperationType.Import
                && x.Status == OperationStatus.Open)
            .Include(x => x.ImportDetails)
            .Include(x => x.Containers)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FclSuggestion>>
        GetSuggestionsForExportContainerAsync(
            Guid exportContainerId,
            CancellationToken cancellationToken = default)
    {
        return await dbContext.FclSuggestions
            .Where(x => x.ExportContainerId == exportContainerId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddSuggestionsAsync(
        IEnumerable<FclSuggestion> suggestions,
        CancellationToken cancellationToken = default)
    {
        await dbContext.FclSuggestions.AddRangeAsync(
            suggestions,
            cancellationToken);
    }
}
