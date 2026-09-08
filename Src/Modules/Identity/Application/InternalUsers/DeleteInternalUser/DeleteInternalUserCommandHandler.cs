using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.InternalUsers.DeleteInternalUser;

public sealed class DeleteInternalUsersCommandHandler(
    IInternalUserRepository internalUsers,
    IIdentityUserService identityUsers,
    IIdentityUnitOfWork identityUnitOfWork)
    : IRequestHandler<DeleteInternalUsersCommand, Result>
{
    public async Task<Result> Handle(
        DeleteInternalUsersCommand request,
        CancellationToken ct)
    {
        foreach (var internalUserId in request.InternalUserIds)
        {
            var internalUser = await internalUsers.GetByIdAsync(
                internalUserId,
                ct);

            if (internalUser is null)
                return Result.Failure(
                    $"Internal user '{internalUserId}' not found.");

            var identityResult = await identityUsers.DeleteUserAsync(
                internalUser.UserId,
                ct);

            if (!identityResult.Succeeded)
                return Result.Failure(identityResult.Error!);

            internalUsers.Delete(internalUser);
        }

        await identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}