
using Operations.Domain.Entities;

namespace Operations.Application.Abstractions;

public interface IFclRepository
{
    Task<FclSuggestion?> GetSuggestionForApprovalAsync(
        Guid suggestionId,
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

    Task<IReadOnlyList<Operation>> GetImportCandidatesAsync(
        Guid clientId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FclSuggestion>> GetSuggestionsForExportContainerAsync(
        Guid exportContainerId,
        CancellationToken cancellationToken = default);

    Task AddSuggestionsAsync(
        IEnumerable<FclSuggestion> suggestions,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FclSuggestion>> GetAllSuggestionsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FclApproval>> GetAllApprovalsAsync(
    CancellationToken cancellationToken = default);

    void RemoveSuggestions(IEnumerable<FclSuggestion> suggestions);
}
