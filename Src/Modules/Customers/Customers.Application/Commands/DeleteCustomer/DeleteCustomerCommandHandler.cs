using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using MediatR;

namespace Customers.Application.Customers.DeleteCustomer;

public sealed class DeleteCustomerCommandHandler(ICustomerRepository customerRepository, ICustomersUnitOfWork IcustomersUnitOfWorkunitOfWork)
    : IRequestHandler<DeleteCustomerCommand, Result>
{
    public async Task<Result> Handle(
        DeleteCustomerCommand request,
        CancellationToken ct)
    {
        var customer = await customerRepository.GetByIdAsync(
            request.CustomerId,
            ct);

        if (customer is null)
            return Result.Failure("Customer not found.");

        customer.MarkAsDeleted(request.DeletedByUserId);

        await IcustomersUnitOfWorkunitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}