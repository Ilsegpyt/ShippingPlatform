using BuildingBlocks.Application;
using FluentValidation;
using Identity.Application.Abstractions;
using Identity.Domain;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Roles.CreateRole;

public sealed record CreateRoleCommand(string Name, string Description) : IRequest<Result<Guid>>;

public sealed class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public sealed class CreateRoleHandler : IRequestHandler<CreateRoleCommand, Result<Guid>>
{
    private readonly IRoleRepository _roles;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;

    public CreateRoleHandler(IRoleRepository roles, IIdentityUnitOfWork identityUnitOfWork)
    {
        _roles = roles;
        _identityUnitOfWork = identityUnitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken ct)
    {
        var existing = await _roles.GetByNameAsync(request.Name, ct);
        if (existing is not null)
            return Result.Failure<Guid>($"A role named '{request.Name}' already exists.");

        var role = Role.Create(request.Name, request.Description);
        _roles.Add(role);
        await _identityUnitOfWork.SaveChangesAsync(ct);

        return Result.Success(role.Id);
    }
}
