using Operations.Domain.Entities;

namespace Operations.Application.Abstractions;

public interface IFclRepository
{
    Task<OperationContainer?> GetImportContainerForApprovalAsync(
        Guid containerId,
        CancellationToken cancellationToken = default);

    Task<bool> IsImportContainerApprovedAsync(
        Guid importContainerId,
        CancellationToken cancellationToken = default);

    Task<HashSet<Guid>> GetApprovedImportContainerIdsAsync(
        CancellationToken cancellationToken = default);

    Task<bool> ApprovalKeyExistsAsync(
        string approvalKey,
        CancellationToken cancellationToken = default);

    Task AddApprovalAsync(
        FclApproval approval,
        CancellationToken cancellationToken = default);

    Task<OperationContainer?> GetExportContainerForMatchingAsync(
        Guid exportContainerId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OperationContainer>> GetAllExportContainersForMatchingAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Operation>> GetAllImportCandidatesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Operation>> GetImportCandidatesAsync(
        Guid clientId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FclApproval>> GetAllApprovalsAsync(
        CancellationToken cancellationToken = default);
}
