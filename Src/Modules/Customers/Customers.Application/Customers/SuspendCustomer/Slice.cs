using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using MediatR;

namespace Customers.Application.Customers.SuspendCustomer;

public sealed record SuspendCustomerCommand(Guid CustomerId) : IRequest<Result>;

public sealed class SuspendCustomerCommandHandler(ICustomerRepository repository, ICustomersUnitOfWork iCustomersUnitOfWork)
    : IRequestHandler<SuspendCustomerCommand, Result>
{
    public async Task<Result> Handle(SuspendCustomerCommand cmd, CancellationToken ct)
    {
        var customer = await repository.GetByIdAsync(cmd.CustomerId, ct);
        if (customer is null) return Result.Failure("Customer not found.");

        customer.Suspend();
        await iCustomersUnitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}