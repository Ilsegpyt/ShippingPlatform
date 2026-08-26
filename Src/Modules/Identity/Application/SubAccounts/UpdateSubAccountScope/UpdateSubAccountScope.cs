using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using Identity.Domain;
using Identity.Domain.Exceptions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.SubAccounts.UpdateSubAccountScope;

public sealed record AddScopeCommand(
    Guid OrganizationId,
    Guid SubAccountId,
    ScopeCategory Category,
    ScopeService Service,
    ScopeShipmentType Type) : IRequest<Result>;


public sealed record RemoveScopeCommand(
    Guid OrganizationId,
    Guid SubAccountId,
    ScopeCategory Category,
    ScopeService Service,
    ScopeShipmentType Type) : IRequest<Result>;



public sealed class AddScopeHandler
    : IRequestHandler<AddScopeCommand, Result>
{
    private readonly ISubAccountRepository _subAccounts;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;

    public AddScopeHandler(
        ISubAccountRepository subAccounts,
        IIdentityUnitOfWork identityUnitOfWork)
    {
        _subAccounts = subAccounts;
        _identityUnitOfWork = identityUnitOfWork;
    }

    public async Task<Result> Handle(
        AddScopeCommand request,
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

public sealed class RemoveScopeHandler
    : IRequestHandler<RemoveScopeCommand, Result>
{
    private readonly ISubAccountRepository _subAccounts;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;

    public RemoveScopeHandler(
        ISubAccountRepository subAccounts,
        IIdentityUnitOfWork IdentityUnitOfWork)
    {
        _subAccounts = subAccounts;
        _identityUnitOfWork = IdentityUnitOfWork;
    }

    public async Task<Result> Handle(
        RemoveScopeCommand request,
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