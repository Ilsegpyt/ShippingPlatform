using BuildingBlocks.Application;
using Customers.Contracts;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.SubAccounts.UpdateSubAccountProfile;

public sealed class UpdateSubAccountProfileHandler(
    ISubAccountRepository subAccounts,
    IIdentityUnitOfWork identityUnitOfWork,
    ICustomerQueries customerQueries)
    : IRequestHandler<UpdateSubAccountProfileCommand, Result>
{
    public async Task<Result> Handle(
        UpdateSubAccountProfileCommand request,
        CancellationToken ct)
    {
        var customer = await customerQueries.GetByIdAsync(
            request.CustomerId,
            ct);

        if (customer is null ||
            customer.IsDeleted ||
            !customer.IsActive)
        {
            return Result.Failure(
                "Only active customers can manage sub-accounts.");
        }

        var subAccount = await subAccounts.GetByIdAsync(
            request.SubAccountId,
            ct);

        if (subAccount is null)
            return Result.Failure("Sub-account not found.");

        if (subAccount.CustomerId != request.CustomerId)
            return Result.Failure(
                "Sub-account does not belong to this organization.");

        subAccount.UpdateName(request.Name);

        await identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
