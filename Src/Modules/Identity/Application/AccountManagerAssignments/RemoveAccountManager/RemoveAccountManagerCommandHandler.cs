
using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.AccountManagerAssignments.RemoveAccountManager;

public sealed class RemoveAccountManagerCommandHandler(
    IAccountManagerAssignmentRepository assignmentRepository,
    IIdentityUnitOfWork unitOfWork)
    : IRequestHandler<RemoveAccountManagerCommand, Result>
{
    public async Task<Result> Handle(
        RemoveAccountManagerCommand cmd,
        CancellationToken ct)
    {
        var assignment =
            await assignmentRepository.GetByCustomerIdAsync(
                cmd.CustomerId,
                ct);

        if (assignment is null)
            return Result.Failure(
                "Customer has no Account Manager assigned.");

        assignmentRepository.Delete(assignment);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

