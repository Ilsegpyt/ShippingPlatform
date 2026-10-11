
using BuildingBlocks.Application;
using Customers.Contracts;
using Identity.Application.Abstractions;
using Identity.Domain.Enums;
using Identity.Domain.Exceptions;
using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;
using MediatR;

namespace Identity.Application.SubAccounts.AddRemoveSubAccountScope;

public sealed record AddScopeCommand(
    Guid CustomerId,
    Guid SubAccountId,
    ScopeCategory Category,
    ScopeService Service,
    ScopeShipmentType Type) : IRequest<Result>;

public sealed record RemoveScopeCommand(
    Guid CustomerId,
    Guid SubAccountId,
    ScopeCategory Category,
    ScopeService Service,
    ScopeShipmentType Type) : IRequest<Result>;

public sealed class AddScopeHandler : IRequestHandler<AddScopeCommand, Result>

{
    private readonly ISubAccountRepository _subAccounts;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;
    private readonly ICustomerQueries _customerQueries;

    public AddScopeHandler(
        ISubAccountRepository subAccounts,
        IIdentityUnitOfWork identityUnitOfWork,
        ICustomerQueries customerQueries)
    {
        _subAccounts = subAccounts;
        _identityUnitOfWork = identityUnitOfWork;
        _customerQueries = customerQueries;
    }

    public async Task<Result> Handle(
        AddScopeCommand request,
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

        try
        {
            var scope = PermissionScope.Create(
                request.Category,
                request.Service,
                request.Type);

            subAccount.AddScope(scope);
        }
        catch (IdentityDomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        _subAccounts.Update(subAccount);

        await _identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

public sealed class RemoveScopeHandler : IRequestHandler<RemoveScopeCommand, Result>

{
    private readonly ISubAccountRepository _subAccounts;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;
    private readonly ICustomerQueries _customerQueries;

    public RemoveScopeHandler(
        ISubAccountRepository subAccounts,
        IIdentityUnitOfWork identityUnitOfWork,
        ICustomerQueries customerQueries)
    {
        _subAccounts = subAccounts;
        _identityUnitOfWork = identityUnitOfWork;
        _customerQueries = customerQueries;
    }

    public async Task<Result> Handle(
        RemoveScopeCommand request,
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

        try
        {
            var scope = PermissionScope.Create(
                request.Category,
                request.Service,
                request.Type);

            subAccount.RemoveScope(scope);
        }
        catch (IdentityDomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        _subAccounts.Update(subAccount);

        await _identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
