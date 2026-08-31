using BuildingBlocks.Application;
using BuildingBlocks.Application.Contracts;
using Identity.Application.Abstractions;
using Identity.Domain;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.AccountManagerAssignments.AssignAccountManager;

public sealed class AssignAccountManagerCommandHandler(
    IInternalUserRepository internalUserRepository,
    IRoleRepository roleRepository,
    IAccountManagerAssignmentRepository assignmentRepository,
    IIdentityUnitOfWork unitOfWork,
    ICustomerQueries customerQueries)
    : IRequestHandler<AssignAccountManagerCommand, Result>
{
    public async Task<Result> Handle(
        AssignAccountManagerCommand cmd,
        CancellationToken ct)
    {
        var internalUser =
            await internalUserRepository.GetByIdAsync(
                cmd.AccountManagerId,
                ct);

        if (internalUser is null)
            return Result.Failure(
                "Account Manager was not found.");

        if (internalUser.Status != InternalUserStatus.Active)
            return Result.Failure(
                "The selected Account Manager is inactive.");

        var role =
            await roleRepository.GetByIdAsync(
                internalUser.RoleId,
                ct);

        if (role is null)
            return Result.Failure(
                "The Account Manager role was not found.");

        if (role.Status != RoleStatus.Active)
            return Result.Failure(
                "The Account Manager role is inactive.");

        if (role.Name != "Account Manager")
            return Result.Failure(
                "The selected user is not an Account Manager.");

        var customer =
            await customerQueries.GetForAssignmentAsync(
                cmd.CustomerId,
                ct);

        if (customer is null)
            return Result.Failure(
                "Customer was not found.");

        if (!customer.IsActive)
            return Result.Failure(
                "Customer is inactive.");

        var existingAssignment =
            await assignmentRepository.GetByCustomerIdAsync(
                cmd.CustomerId,
                ct);

        if (existingAssignment is not null)
            return Result.Failure(
                "Customer is already assigned to an Account Manager.");

        var assignment = AccountManagerAssignment.Create(
            cmd.AccountManagerId,
            cmd.CustomerId);

        assignmentRepository.Add(assignment);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
