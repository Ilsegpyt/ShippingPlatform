using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.InternalUsers.UpdateInternalUserProfile;

public sealed class UpdateInternalUserProfileHandler(
    IInternalUserRepository internalUsers,
    IIdentityUnitOfWork identityUnitOfWork)
    : IRequestHandler<UpdateInternalUserProfileCommand, Result>
{
    public async Task<Result> Handle(
        UpdateInternalUserProfileCommand request,
        CancellationToken ct)
    {
        var internalUser = await internalUsers.GetByIdAsync(
            request.InternalUserId,
            ct);

        if (internalUser is null)
            return Result.Failure("Internal user not found.");

        internalUser.UpdateProfile(
            request.Name,
            request.Phone);

        await identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}