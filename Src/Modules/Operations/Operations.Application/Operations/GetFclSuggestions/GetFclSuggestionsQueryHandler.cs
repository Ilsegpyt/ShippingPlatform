
using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Application.Common;

namespace Operations.Application.Operations.GetFclSuggestions;

public sealed class GetFclSuggestionsQueryHandler(
    IFclRepository repository)
    : IRequestHandler<
        GetFclSuggestionsQuery,
        Result<IReadOnlyList<FclSuggestionDto>>>
{
    public async Task<Result<IReadOnlyList<FclSuggestionDto>>> Handle(
        GetFclSuggestionsQuery query,
        CancellationToken cancellationToken)
    {
        if (query.ExportContainerId == Guid.Empty)
        {
            return Result.Failure<IReadOnlyList<FclSuggestionDto>>(
                "Export container ID is required.");
        }

        var exportContainer = await repository.GetExportContainerForMatchingAsync(
            query.ExportContainerId,
            cancellationToken);

        if (exportContainer is null)
        {
            return Result.Success<IReadOnlyList<FclSuggestionDto>>(Array.Empty<FclSuggestionDto>());
        }

        var imports = await repository.GetAllImportCandidatesAsync(cancellationToken);
        var approvedImportContainerIds = await repository.GetApprovedImportContainerIdsAsync(cancellationToken);

        var suggestions = FclMatchingEngine.GenerateSuggestions(
            imports,
            new[] { exportContainer },
            approvedImportContainerIds);

        return Result.Success(suggestions);
    }
}
