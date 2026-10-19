
using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.GenerateFclSuggestions;

public sealed record GenerateFclSuggestionsCommand(
    Guid ExportContainerId
) : IRequest<Result<int>>;
