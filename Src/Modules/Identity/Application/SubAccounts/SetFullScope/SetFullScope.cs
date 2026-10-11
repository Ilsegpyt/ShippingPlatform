
using BuildingBlocks.Application;
using Customers.Contracts;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.SubAccounts.SetFullScope;

public sealed record SetFullScopeCommand(
    Guid CustomerId,
    Guid SubAccountId) : IRequest<Result>;

public sealed class SetFullScopeHandler : IRequestHandler<SetFullScopeCommand, Result>
{
    private readonly ISubAccountRepository _subAccounts;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;
    private readonly ICustomerQueries _customerQueries;

    public SetFullScopeHandler(
        ISubAccountRepository subAccounts,
        IIdentityUnitOfWork identityUnitOfWork,
        ICustomerQueries customerQueries)
    {
        _subAccounts = subAccounts;
        _identityUnitOfWork = identityUnitOfWork;
        _customerQueries = customerQueries;
    }

    public async Task<Result> Handle(
        SetFullScopeCommand request,
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

        subAccount.SetFullScope();

        _subAccounts.Update(subAccount);

        await _identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
