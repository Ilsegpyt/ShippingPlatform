using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.Me.GetMe;

public sealed record GetMeQuery(Guid UserId)
    : IRequest<Result<MeResponse>>;

public sealed record MeResponse(
    Guid UserId,
    Guid ProfileId,
    string TokenType,
    Guid? OrganizationId,
    string Name,
    string Email,
    string? Phone,
    string? CompanyName,
    string? RoleName,
    IReadOnlyCollection<string> Permissions);