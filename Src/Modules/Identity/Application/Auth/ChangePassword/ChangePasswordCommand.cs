using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using MediatR;

namespace Identity.Application.Auth.ChangePassword;

public sealed record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword)
    : IRequest<Result<ChangePasswordResponse>>;

public sealed record ChangePasswordResponse(bool Changed);

public sealed class ChangePasswordHandler
    : IRequestHandler<
        ChangePasswordCommand,
        Result<ChangePasswordResponse>>
{
    private readonly IIdentityUserService _identityUsers;

    public ChangePasswordHandler(
        IIdentityUserService identityUsers)
    {
        _identityUsers = identityUsers;
    }

    public async Task<Result<ChangePasswordResponse>> Handle(
        ChangePasswordCommand request,
        CancellationToken ct)
    {
        var result = await _identityUsers.UpdatePasswordAsync(
            request.UserId,
            request.CurrentPassword,
            request.NewPassword,
            ct);

        if (!result.Succeeded)
        {
            return Result.Failure<ChangePasswordResponse>(
                result.Error ?? "Password change failed.");
        }

        return Result.Success(
            new ChangePasswordResponse(true));
    }
}