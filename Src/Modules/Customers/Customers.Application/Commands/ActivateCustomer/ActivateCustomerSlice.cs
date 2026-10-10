using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using MediatR;

namespace Customers.Application.Customers.ActivateCustomer;

public sealed record ActivateCustomerCommand(Guid CustomerId) : IRequest<Result>;

public sealed class ActivateCustomerCommandHandler(ICustomerRepository repository, ICustomersUnitOfWork iCustomersUnitOfWork)
    : IRequestHandler<ActivateCustomerCommand, Result>
{
    public async Task<Result> Handle(ActivateCustomerCommand cmd, CancellationToken ct)
    {
        var customer = await repository.GetByIdAsync(cmd.CustomerId, ct);
        if (customer is null) 
            return Result.Failure("Customer not found.");

        customer.Activate();
        await iCustomersUnitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}