using BuildingBlocks.Application;
using FluentValidation;
using Identity.Application.Abstractions;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.InternalUsers.CreateInternalUser;



public sealed class CreateInternalUserCommandHandler
    : IRequestHandler<CreateInternalUserCommand, Result<CreateInternalUserResponse>>
{
    private readonly IIdentityUserService _identityUsers;
    private readonly IInternalUserRepository _internalUsers;
    private readonly IRoleRepository _roles;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;

   

    public CreateInternalUserCommandHandler(
        IIdentityUserService identityUsers,
        IInternalUserRepository internalUsers,
        IRoleRepository roles,
        IIdentityUnitOfWork identityUnitOfWork
    )
    {
        _identityUsers = identityUsers;
        _internalUsers = internalUsers;
        _roles = roles;
        _identityUnitOfWork = identityUnitOfWork;

    }

    public async Task<Result<CreateInternalUserResponse>> Handle(
        CreateInternalUserCommand command,
        CancellationToken ct)
    {
        var role = await _roles.GetByIdAsync(command.RoleId, ct);

        if (role is null)
        {
            return Result.Failure<CreateInternalUserResponse>(
                "Role not found.");
        }

        await _identityUnitOfWork.BeginTransactionAsync(ct);

        try
        {
            var userId = await _identityUsers.CreateUserAsync(
                command.Email,
                isInternal: true,
                command.Phone,
                ct);

            var internalUser = InternalUser.Create(
                userId,
                command.RoleId,
                command.Name,
                command.Email,
                command.Phone);

            _internalUsers.Add(internalUser);

            await _identityUnitOfWork.SaveChangesAsync(ct);

            await _identityUnitOfWork.CommitTransactionAsync(ct);

            return Result.Success(
                new CreateInternalUserResponse(
                    internalUser.Id));
        }
        catch
        {
            await _identityUnitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
    }
}