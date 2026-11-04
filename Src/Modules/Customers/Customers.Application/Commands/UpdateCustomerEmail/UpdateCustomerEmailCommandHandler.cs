using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using Customers.Domain.Entities;
using MediatR;

namespace Customers.Application.Commands.UpdateCustomerEmail;

public sealed class UpdateCustomerEmailCommandHandler(
    ICustomerRepository repository,
    ICustomersUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCustomerEmail, Result>
{
    public async Task<Result> Handle(UpdateCustomerEmail command, CancellationToken ct)
    {
        var customer = await repository.GetByIdAsync(command.CustomerId, ct);

        if (customer is null)
            return Result.Failure("Customer not found.");

        if (customer.IsDeleted)
            return Result.Failure("Deleted customers cannot update their email.");

        if (customer.Status != CustomerStatus.Active)
            return Result.Failure("Inactive customers cannot update their email.");

        customer.UpdateEmail(command.Email);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
