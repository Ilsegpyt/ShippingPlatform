using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Operations.GetFclSuggestions;

namespace Operations.Application.Operations.GetAllFclSuggestions;

public sealed record GetAllFclSuggestionsQuery
    : IRequest<Result<IReadOnlyList<FclSuggestionDto>>>;