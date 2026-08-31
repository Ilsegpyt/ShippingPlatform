using BuildingBlocks.Application;
using BuildingBlocks.Application.Contracts;
using Identity.Application.Abstractions;
using Identity.Domain;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.AccountManagerAssignments.ChangeAccountManager;

public sealed class ChangeAccountManagerCommandHandler(
    IInternalUserRepository internalUserRepository,
    IRoleRepository roleRepository,
    IAccountManagerAssignmentRepository assignmentRepository,
    IIdentityUnitOfWork unitOfWork,
    ICustomerQueries customerQueries)
    : IRequestHandler<ChangeAccountManagerCommand, Result>
{
    public async Task<Result> Handle(
        ChangeAccountManagerCommand cmd,
        CancellationToken ct)
    {
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

        var assignment =
            await assignmentRepository.GetByCustomerIdAsync(
                cmd.CustomerId,
                ct);

        if (assignment is null)
            return Result.Failure(
                "Customer has no Account Manager assigned.");

        var internalUser =
            await internalUserRepository.GetByIdAsync(
                cmd.NewAccountManagerId,
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

        if (assignment.AccountManagerId == cmd.NewAccountManagerId)
            return Result.Failure(
                "This Account Manager is already assigned to the customer.");

        assignment.ChangeAccountManager(
            cmd.NewAccountManagerId);

        assignmentRepository.Update(assignment);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}