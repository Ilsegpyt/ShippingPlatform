using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.Me.GetMe;

public sealed record GetMeQuery(Guid UserId)
    : IRequest<Result<MeResponse>>;

public sealed record MeResponse(
    Guid UserId,
    string TokenType,
    Guid? OrganizationId,
    IReadOnlyCollection<string> Permissions);