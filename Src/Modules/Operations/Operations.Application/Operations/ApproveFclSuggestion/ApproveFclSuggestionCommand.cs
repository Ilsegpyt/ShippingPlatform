using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.ApproveFclSuggestion;

/// <param name="ConfirmNearMatch">Must be true when the suggestion matches exactly 5 of 6 criteria.</param>
public sealed record ApproveFclSuggestionCommand(Guid SuggestionId, bool ConfirmNearMatch)
    : IRequest<Result<Guid>>;
