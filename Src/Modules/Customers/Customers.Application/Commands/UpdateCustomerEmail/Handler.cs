using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using MediatR;

namespace Customers.Application.Customers.UpdateCustomerEmail;

public sealed class UpdateCustomerEmailHandler(
    ICustomerRepository repository,
    ICustomersUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCustomerEmail, Result>
{
    public async Task<Result> Handle(UpdateCustomerEmail command, CancellationToken ct)
    {
        var customer = await repository.GetByIdAsync(command.CustomerId, ct);

        if (customer is null)
            return Result.Failure("Customer not found.");

        customer.UpdateEmail(command.Email);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}