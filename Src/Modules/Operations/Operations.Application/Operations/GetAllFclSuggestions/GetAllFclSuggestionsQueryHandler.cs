using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Application.Common;
using Operations.Application.Operations.GetFclSuggestions;

namespace Operations.Application.Operations.GetAllFclSuggestions;

public sealed class GetAllFclSuggestionsQueryHandler(
    IFclRepository repository)
    : IRequestHandler<
        GetAllFclSuggestionsQuery,
        Result<IReadOnlyList<FclSuggestionDto>>>
{
    public async Task<Result<IReadOnlyList<FclSuggestionDto>>> Handle(
        GetAllFclSuggestionsQuery query,
        CancellationToken cancellationToken)
    {
        var imports = await repository.GetAllImportCandidatesAsync(cancellationToken);
        var exportContainers = await repository.GetAllExportContainersForMatchingAsync(cancellationToken);
        var approvedImportContainerIds = await repository.GetApprovedImportContainerIdsAsync(cancellationToken);

        var suggestions = FclMatchingEngine.GenerateSuggestions(
            imports,
            exportContainers,
            approvedImportContainerIds);

        return Result.Success(suggestions);
    }
}