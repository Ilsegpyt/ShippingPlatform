using BuildingBlocks.Application;
using Identity.Domain;
using MediatR;

namespace Identity.Application.SubAccounts.CreateSubAccount;

public sealed record ScopeInput(ScopeCategory Category, ScopeService Service, ScopeShipmentType Type);

public sealed record CreateSubAccountCommand(
    Guid OrganizationId,
    string Name,
    string Email,
    bool GrantFullScope,
    IReadOnlyList<ScopeInput> Scopes) : IRequest<Result<CreateSubAccountResponse>>;

public sealed record CreateSubAccountResponse(Guid SubAccountId, string DefaultPassword);