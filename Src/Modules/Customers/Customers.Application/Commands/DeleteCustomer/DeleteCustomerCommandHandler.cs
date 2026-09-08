using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using MediatR;

namespace Customers.Application.Commands.DeleteCustomer;

public sealed class DeleteCustomerCommandHandler(ICustomerRepository customerRepository, ICustomersUnitOfWork  icustomersUnitOfWorkunitOfWork)
    : IRequestHandler<DeleteCustomersCommand, Result>
{
    public async Task<Result> Handle(
     DeleteCustomersCommand request,
     CancellationToken ct)
    {
        foreach (var customerId in request.CustomerIds)
        {
            var customer = await customerRepository.GetByIdAsync(
                customerId,
                ct);

            if (customer is null)
                return Result.Failure($"Customer '{customerId}' not found.");

            customer.MarkAsDeleted(request.DeletedByUserId);
        }

        await icustomersUnitOfWorkunitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
 }