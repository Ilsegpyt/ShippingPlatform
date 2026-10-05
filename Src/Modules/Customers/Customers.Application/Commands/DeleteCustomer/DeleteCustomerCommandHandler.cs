using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using Customers.Application.Abstractions;
using MediatR;

namespace Customers.Application.Commands.DeleteCustomer;

public sealed class DeleteCustomerCommandHandler(
    ICustomerRepository customerRepository,
    ICustomersUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<DeleteCustomersCommand, Result>
{
    public async Task<Result> Handle(
        DeleteCustomersCommand request,
        CancellationToken ct)
    {
        var userId = currentUser.UserId;
        var userName = await currentUser.GetUserNameAsync(ct);
        var now = DateTime.UtcNow;

        foreach (var customerId in request.CustomerIds)
        {
            var customer = await customerRepository.GetByIdAsync(
                customerId,
                ct);

            if (customer is null)
                return Result.Failure(
                    $"Customer '{customerId}' not found.");

            customer.MarkAsDeleted(
                userId,
                userName,
                now);
        }

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}