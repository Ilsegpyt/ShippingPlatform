using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.SubAccounts.ResetSubAccountPassword;

public sealed record ResetSubAccountPasswordCommand(
    Guid OrganizationId,
    Guid SubAccountId,
    string NewPassword) : IRequest<Result>;

public sealed class ResetSubAccountPasswordHandler
    : IRequestHandler<ResetSubAccountPasswordCommand, Result>
{
    private readonly ISubAccountRepository _subAccounts;
    private readonly IIdentityUserService _identityUsers;

    public ResetSubAccountPasswordHandler(
        ISubAccountRepository subAccounts,
        IIdentityUserService identityUsers)
    {
        _subAccounts = subAccounts;
        _identityUsers = identityUsers;
    }

    public async Task<Result> Handle(
        ResetSubAccountPasswordCommand request,
        CancellationToken ct)
    {
        var subAccount = await _subAccounts.GetByIdAsync(
            request.SubAccountId,
            ct);

        if (subAccount is null)
            return Result.Failure("Sub-account not found.");

        if (subAccount.OrganizationId != request.OrganizationId)
            return Result.Failure(
                "Sub-account does not belong to this organization.");

        return await _identityUsers.ResetPasswordAsync(
            subAccount.UserId,
            request.NewPassword,
            ct);
    }
}
