using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using MediatR;

namespace Identity.Application.Auth.ResetPassword;

public sealed class ResetPasswordCommandHandler
    : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IIdentityUserService _identityUsers;

    public ResetPasswordCommandHandler(
        IIdentityUserService identityUsers)
    {
        _identityUsers = identityUsers;
    }

    public async Task<Result> Handle(
        ResetPasswordCommand request,
        CancellationToken ct)
    {
        var result = await _identityUsers.ResetPasswordWithTokenAsync(
            request.UserId,
            request.ResetToken,
            request.NewPassword,
            ct);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Error!);
    }
}