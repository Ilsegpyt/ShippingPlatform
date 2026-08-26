using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.SubAccounts.ActivateSubAccount;

public sealed record ActivateSubAccountCommand(
    Guid OrganizationId,
    Guid SubAccountId) : IRequest<Result>;

public sealed record DeactivateSubAccountCommand(
    Guid OrganizationId,
    Guid SubAccountId) : IRequest<Result>;

public sealed class ActivateSubAccountHandler
    : IRequestHandler<ActivateSubAccountCommand, Result>
{
    private readonly ISubAccountRepository _subAccounts;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;

    public ActivateSubAccountHandler(
        ISubAccountRepository subAccounts,
        IIdentityUnitOfWork identityUnitOfWork)
    {
        _subAccounts = subAccounts;
        _identityUnitOfWork = identityUnitOfWork;
    }

    public async Task<Result> Handle(
        ActivateSubAccountCommand request,
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

        subAccount.Activate();

        _subAccounts.Update(subAccount);
        await _identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

public sealed class DeactivateSubAccountHandler
    : IRequestHandler<DeactivateSubAccountCommand, Result>
{
    private readonly ISubAccountRepository _subAccounts;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;

    public DeactivateSubAccountHandler(
        ISubAccountRepository subAccounts,
        IIdentityUnitOfWork identityUnitOfWork)
    {
        _subAccounts = subAccounts;
        _identityUnitOfWork = identityUnitOfWork;
    }

    public async Task<Result> Handle(
        DeactivateSubAccountCommand request,
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

        subAccount.Deactivate();

        _subAccounts.Update(subAccount);
        await _identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
