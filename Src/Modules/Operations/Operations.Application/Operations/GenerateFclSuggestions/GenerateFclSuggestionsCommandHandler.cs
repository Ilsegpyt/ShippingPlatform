using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Application.Common;
using Operations.Application.Operations.GetFclSuggestions;

namespace Operations.Application.Operations.GenerateFclSuggestions;

public sealed class GenerateFclSuggestionsCommandHandler(
    IFclRepository repository)
    : IRequestHandler<GenerateFclSuggestionsCommand, Result<IReadOnlyList<FclSuggestionDto>>>
{
    public async Task<Result<IReadOnlyList<FclSuggestionDto>>> Handle(
        GenerateFclSuggestionsCommand command,
        CancellationToken ct)
    {
        var imports = await repository.GetAllImportCandidatesAsync(ct);
        var exportContainers = await repository.GetAllExportContainersForMatchingAsync(ct);
        var approvedImportContainerIds = await repository.GetApprovedImportContainerIdsAsync(ct);

        if (command.ExportContainerId.HasValue)
        {
            var targetId = command.ExportContainerId.Value;
            var filtered = exportContainers
                .Where(c => c.Id == targetId)
                .ToList();

            if (filtered.Count == 0)
            {
                var target = await repository.GetExportContainerForMatchingAsync(targetId, ct);
                if (target is null)
                {
                    return Result.Failure<IReadOnlyList<FclSuggestionDto>>(
                        "Open Export container without a container number, with loading date, time and type, was not found.");
                }

                exportContainers = new[] { target };
            }
            else
            {
                exportContainers = filtered;
            }
        }

        var suggestions = FclMatchingEngine.GenerateSuggestions(
            imports,
            exportContainers,
            approvedImportContainerIds);

        return Result.Success(suggestions);
    }
}
