using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.InternalUsers.ChangeInternalUserStatus;

public sealed class ChangeInternalUserStatusCommandHandler(
    IInternalUserRepository internalUsers,
    IIdentityUnitOfWork identityUnitOfWork)
    : IRequestHandler<ChangeInternalUserStatusCommand, Result>
{
    public async Task<Result> Handle(
        ChangeInternalUserStatusCommand request,
        CancellationToken ct)
    {
        var internalUser = await internalUsers.GetByIdAsync(
            request.InternalUserId,
            ct);

        if (internalUser is null)
            return Result.Failure("Internal user not found.");

        switch (request.Action)
        {
            case InternalUserStatusAction.Activate:
                internalUser.Activate();
                break;

            case InternalUserStatusAction.Deactivate:
                internalUser.Deactivate();
                break;

            default:
                return Result.Failure("Invalid status action.");
        }

        await identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}