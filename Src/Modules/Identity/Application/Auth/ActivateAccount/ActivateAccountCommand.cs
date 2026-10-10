
using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.Auth.ActivateAccount;

public sealed record ActivateAccountCommand(
    Guid UserId,
    string ActivationToken,
    string NewPassword) : IRequest<Result<ActivateAccountResponse>>;


public sealed record ActivateAccountResponse(
    bool Activated);
