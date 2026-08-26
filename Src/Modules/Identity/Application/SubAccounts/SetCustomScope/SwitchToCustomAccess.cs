using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.SubAccounts.SetCustomScope;

public sealed record SetCustomScopeCommand(
    Guid OrganizationId,
    Guid SubAccountId) : IRequest<Result>;

public sealed class SetCustomScopeHandler
    : IRequestHandler<SetCustomScopeCommand, Result>
{
    private readonly ISubAccountRepository _subAccounts;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;

    public SetCustomScopeHandler(
        ISubAccountRepository subAccounts,
        IIdentityUnitOfWork identityUnitOfWork)
    {
        _subAccounts = subAccounts;
        _identityUnitOfWork = identityUnitOfWork;
    }

    public async Task<Result> Handle(
        SetCustomScopeCommand request,
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

        subAccount.SetCustomScope();

        _subAccounts.Update(subAccount);

        await _identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}