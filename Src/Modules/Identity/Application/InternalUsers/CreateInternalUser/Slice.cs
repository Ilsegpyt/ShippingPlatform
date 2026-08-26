using BuildingBlocks.Application;
using FluentValidation;
using Identity.Application.Abstractions;
using Identity.Domain;
using Identity.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Options;

namespace Identity.Application.InternalUsers.CreateInternalUser;

public sealed record CreateInternalUserCommand(string Name, string Email, string Phone, Guid RoleId) : IRequest<Result<CreateInternalUserResponse>>;

public sealed record CreateInternalUserResponse(Guid InternalUserId, string DefaultPassword);

public sealed class CreateInternalUserValidator : AbstractValidator<CreateInternalUserCommand>
{
    public CreateInternalUserValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.RoleId).NotEmpty();
    }
}

public sealed class CreateInternalUserHandler : IRequestHandler<CreateInternalUserCommand, Result<CreateInternalUserResponse>>
{
    private readonly IIdentityUserService _identityUsers;
    private readonly IInternalUserRepository _internalUsers;
    private readonly IRoleRepository _roles;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;
    private readonly SubAccountOptions _options; // reuses the same "default password" policy

    public CreateInternalUserHandler(
        IIdentityUserService identityUsers,
        IInternalUserRepository internalUsers,
        IRoleRepository roles,
        IIdentityUnitOfWork identityUnitOfWork,
        IOptions<SubAccountOptions> options)
    {
        _identityUsers = identityUsers;
        _internalUsers = internalUsers;
        _roles = roles;
        _identityUnitOfWork = identityUnitOfWork;
        _options = options.Value;
    }

    public async Task<Result<CreateInternalUserResponse>> Handle(CreateInternalUserCommand command, CancellationToken ct)
    {
        var role = await _roles.GetByIdAsync(command.RoleId, ct);
        if (role is null)
            return Result.Failure<CreateInternalUserResponse>("Role not found.");

        var userId = await _identityUsers.CreateUserAsync(command.Email, _options.DefaultPassword, isInternal: true,command.Phone, ct);

        var internalUser = InternalUser.Create(userId, command.RoleId ,command.Name ,command.Email, command.Phone);
        _internalUsers.Add(internalUser);
        await _identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success(new CreateInternalUserResponse(internalUser.Id, _options.DefaultPassword));
    }
}

