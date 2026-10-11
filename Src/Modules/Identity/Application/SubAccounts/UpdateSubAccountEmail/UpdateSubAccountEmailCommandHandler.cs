
using BuildingBlocks.Application;
using Customers.Contracts;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.SubAccounts.UpdateSubAccountEmail;

public sealed class UpdateSubAccountEmailCommandHandler(
    ISubAccountRepository repository,
    IIdentityUserService identityUserService,
    IIdentityUnitOfWork unitOfWork,
    ICustomerQueries customerQueries)
    : IRequestHandler<UpdateSubAccountEmailCommand, Result>
{
    public async Task<Result> Handle(
        UpdateSubAccountEmailCommand command,
        CancellationToken ct)
    {
        var customer = await customerQueries.GetByIdAsync(
            command.CustomerId,
            ct);

        if (customer is null ||
            customer.IsDeleted ||
            !customer.IsActive)
        {
            return Result.Failure(
                "Only active customers can manage sub-accounts.");
        }

        var subAccount = await repository.GetByIdAsync(
            command.SubAccountId,
            ct);

        if (subAccount is null)
            return Result.Failure("Sub-account not found.");

        if (subAccount.CustomerId != command.CustomerId)
            return Result.Failure(
                "Sub-account does not belong to this organization.");

        var identityResult = await identityUserService.UpdateEmailAsync(
            subAccount.UserId,
            command.Email,
            ct);

        if (!identityResult.Succeeded)
            return Result.Failure(identityResult.Error!);

        subAccount.UpdateEmail(command.Email);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
