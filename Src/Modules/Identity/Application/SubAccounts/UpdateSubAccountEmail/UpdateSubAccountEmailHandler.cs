using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.SubAccounts.UpdateSubAccountEmail;

public sealed class UpdateSubAccountEmailHandler(
    ISubAccountRepository repository,
    IIdentityUserService identityUserService,
    IIdentityUnitOfWork unitOfWork)
    : IRequestHandler<UpdateSubAccountEmailCommand, Result>
{
    public async Task<Result> Handle(
        UpdateSubAccountEmailCommand command,
        CancellationToken ct)
    {
        var subAccount = await repository.GetByIdAsync(
            command.SubAccountId,
            ct);

        if (subAccount is null)
            return Result.Failure("Sub-account not found.");

        var identityResult = await identityUserService.UpdateEmailAsync(
            subAccount.UserId,
            command.Email,
            ct);

        if (!identityResult.IsSuccess)
            return identityResult;

        subAccount.UpdateEmail(command.Email);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}