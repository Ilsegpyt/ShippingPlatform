using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.InternalUsers.UpdateInternalUserEmail;

public sealed class UpdateInternalUserEmailHandler(
    IInternalUserRepository internalUsers,
    IIdentityUserService identityUserService,
    IIdentityUnitOfWork identityUnitOfWork)
    : IRequestHandler<UpdateInternalUserEmailCommand, Result>
{
    public async Task<Result> Handle(
        UpdateInternalUserEmailCommand request,
        CancellationToken ct)
    {
        var internalUser = await internalUsers.GetByIdAsync(
            request.InternalUserId,
            ct);

        if (internalUser is null)
            return Result.Failure("Internal user not found.");

        var identityResult = await identityUserService.UpdateEmailAsync(
            internalUser.UserId,
            request.Email,
            ct);

        if (identityResult.IsFailure)
            return identityResult;

        internalUser.UpdateEmail(request.Email);

        await identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}