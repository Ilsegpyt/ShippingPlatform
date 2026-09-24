using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using MediatR;

namespace Identity.Application.Auth.ActivateAccount;

public sealed record ActivateAccountCommand(
    Guid UserId,
    string ActivationToken,
    string NewPassword) : IRequest<Result<ActivateAccountResponse>>;

public sealed record ActivateAccountResponse(
    bool Activated);

public sealed class ActivateAccountHandler
    : IRequestHandler<
        ActivateAccountCommand,
        Result<ActivateAccountResponse>>
{
    private readonly IIdentityUserService _identityUsers;

    public ActivateAccountHandler(
        IIdentityUserService identityUsers)
    {
        _identityUsers = identityUsers;
    }

    public async Task<Result<ActivateAccountResponse>> Handle(
        ActivateAccountCommand request,
        CancellationToken ct)
    {
        var result = await _identityUsers.ActivateUserAsync(
            request.UserId,
            request.ActivationToken,
            request.NewPassword,
            ct);

        if (!result.Succeeded)
        {
            return Result.Failure<ActivateAccountResponse>(
                result.Error ?? "Account activation failed.");
        }

        return Result.Success(
            new ActivateAccountResponse(true));
    }
}