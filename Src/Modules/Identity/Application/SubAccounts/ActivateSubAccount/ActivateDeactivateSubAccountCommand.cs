
using BuildingBlocks.Application;
using Customers.Contracts;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.SubAccounts.ActivateSubAccount;

public sealed record ActivateSubAccountCommand(
    Guid CustomerId,
    Guid SubAccountId) : IRequest<Result>;

public sealed record DeactivateSubAccountCommand(
    Guid CustomerId,
    Guid SubAccountId) : IRequest<Result>;

public sealed class ActivateSubAccountHandler
    : IRequestHandler<ActivateSubAccountCommand, Result>
{
    private readonly ISubAccountRepository _subAccounts;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;
    private readonly ICustomerQueries _customerQueries;

    public ActivateSubAccountHandler(
        ISubAccountRepository subAccounts,
        IIdentityUnitOfWork identityUnitOfWork,
        ICustomerQueries customerQueries)
    {
        _subAccounts = subAccounts;
        _identityUnitOfWork = identityUnitOfWork;
        _customerQueries = customerQueries;
    }

    public async Task<Result> Handle(
        ActivateSubAccountCommand request,
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
    private readonly ICustomerQueries _customerQueries;

    public DeactivateSubAccountHandler(
        ISubAccountRepository subAccounts,
        IIdentityUnitOfWork identityUnitOfWork,
        ICustomerQueries customerQueries)
    {
        _subAccounts = subAccounts;
        _identityUnitOfWork = identityUnitOfWork;
        _customerQueries = customerQueries;
    }

    public async Task<Result> Handle(
        DeactivateSubAccountCommand request,
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

        subAccount.Deactivate();

        _subAccounts.Update(subAccount);
        await _identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
