using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Operations.GetFclSuggestions;

namespace Operations.Application.Operations.GenerateFclSuggestions;

/// <summary>
/// When ExportContainerId is null, generates suggestions across all eligible Import
/// containers and unnumbered Export containers dynamically in memory.
/// </summary>
public sealed record GenerateFclSuggestionsCommand(
    Guid? ExportContainerId = null
) : IRequest<Result<IReadOnlyList<FclSuggestionDto>>>;
