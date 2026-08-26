using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using Identity.Domain;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.SubAccounts.RevokeSubAccountPermission;

public sealed record RevokeSubAccountPermissionCommand(
    Guid OrganizationId,
    Guid SubAccountId,
    string PermissionKey) : IRequest<Result>;

public sealed class RevokeSubAccountPermissionHandler
    : IRequestHandler<RevokeSubAccountPermissionCommand, Result>
{
    private readonly ISubAccountRepository _subAccounts;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;

    public RevokeSubAccountPermissionHandler(
        ISubAccountRepository subAccounts,
        IIdentityUnitOfWork identityUnitOfWork)
    {
        _subAccounts = subAccounts;
        _identityUnitOfWork = identityUnitOfWork;
    }

    public async Task<Result> Handle(
        RevokeSubAccountPermissionCommand request,
        CancellationToken ct)
    {
        var subAccount = await _subAccounts.GetByIdAsync(
            request.SubAccountId,
            ct);

        if (subAccount is null)
            return Result.Failure("Sub-account not found.");

        // Make sure the sub-account belongs to the authenticated user's organization.
        if (subAccount.OrganizationId != request.OrganizationId)
            return Result.Failure(
                "Sub-account does not belong to this organization.");

        var key = PermissionKey.Of(request.PermissionKey);

        if (!PermissionCatalog.SubAccountPermissions.Contains(key))
            return Result.Failure(
                $"'{request.PermissionKey}' cannot be assigned to a sub-account.");

        subAccount.RevokePermission(key);

        _subAccounts.Update(subAccount);
        await _identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}