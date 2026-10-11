
using BuildingBlocks.Application;
using Customers.Contracts;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;
using MediatR;

namespace Identity.Application.SubAccounts.GrantSubAccountPermission;

public sealed record GrantSubAccountPermissionCommand(
    Guid CustomerId,
    Guid SubAccountId,
    string PermissionKey) : IRequest<Result>;

public sealed class GrantSubAccountPermissionHandler
    : IRequestHandler<GrantSubAccountPermissionCommand, Result>
{
    private readonly ISubAccountRepository _subAccounts;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;
    private readonly ICustomerQueries _customerQueries;

    public GrantSubAccountPermissionHandler(
        ISubAccountRepository subAccounts,
        IIdentityUnitOfWork identityUnitOfWork,
        ICustomerQueries customerQueries)
    {
        _subAccounts = subAccounts;
        _identityUnitOfWork = identityUnitOfWork;
        _customerQueries = customerQueries;
    }

    public async Task<Result> Handle(
        GrantSubAccountPermissionCommand request,
        CancellationToken ct)
    {
        var customer = await _customerQueries.GetByIdAsync(
            request.CustomerId,
            ct);

        if (customer is null ||
            customer.IsDeleted ||
            !customer.IsActive)
        {
            return Result.Failure(
                "Only active customers can manage sub-accounts.");
        }

        var subAccount = await _subAccounts.GetByIdAsync(
            request.SubAccountId,
            ct);

        if (subAccount is null)
            return Result.Failure("Sub-account not found.");

        if (subAccount.CustomerId != request.CustomerId)
            return Result.Failure(
                "Sub-account does not belong to this organization.");

        var key = PermissionKey.Of(request.PermissionKey);

        if (!PermissionCatalog.SubAccountPermissions.Contains(key))
            return Result.Failure(
                $"'{request.PermissionKey}' cannot be assigned to a sub-account.");

        subAccount.GrantPermission(key);

        _subAccounts.Update(subAccount);
        await _identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
