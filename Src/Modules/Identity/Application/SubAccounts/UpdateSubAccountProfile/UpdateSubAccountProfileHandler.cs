using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.SubAccounts.UpdateSubAccountProfile;

public sealed class UpdateSubAccountProfileHandler(
    ISubAccountRepository subAccounts,
    IIdentityUnitOfWork identityUnitOfWork)
    : IRequestHandler<UpdateSubAccountProfileCommand, Result>
{
    public async Task<Result> Handle(
        UpdateSubAccountProfileCommand request,
        CancellationToken ct)
    {
        var subAccount = await subAccounts.GetByIdAsync(
            request.SubAccountId,
            ct);

        if (subAccount is null)
            return Result.Failure("Sub-account not found.");

        if (subAccount.OrganizationId != request.OrganizationId)
            return Result.Failure(
                "Sub-account does not belong to this organization.");

        subAccount.UpdateName(request.Name);

        await identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}