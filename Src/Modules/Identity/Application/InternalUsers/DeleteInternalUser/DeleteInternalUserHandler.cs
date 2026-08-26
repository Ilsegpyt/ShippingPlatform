using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.InternalUsers.DeleteInternalUser;

public sealed class DeleteInternalUserHandler(
    IInternalUserRepository internalUsers,
    IIdentityUserService identityUsers,
    IIdentityUnitOfWork identityUnitOfWork)
    : IRequestHandler<DeleteInternalUserCommand, Result>
{
    public async Task<Result> Handle(
        DeleteInternalUserCommand request,
        CancellationToken ct)
    {
        var internalUser = await internalUsers.GetByIdAsync(
            request.InternalUserId,
            ct);

        if (internalUser is null)
            return Result.Failure("Internal user not found.");

        var identityResult = await identityUsers.DeleteUserAsync(
            internalUser.UserId,
            ct);

        if (identityResult.IsFailure)
            return identityResult;

        internalUsers.Delete(internalUser);

        await identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}