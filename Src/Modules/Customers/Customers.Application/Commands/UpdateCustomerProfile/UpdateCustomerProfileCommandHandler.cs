using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using Customers.Application.Commands.UpdateCustomerProfile;
using MediatR;

namespace Customers.Application.Customers.UpdateCustomerProfile;


public sealed class UpdateCustomerProfileCommandHandler(ICustomerRepository repository, ICustomersUnitOfWork iCustomersUnitOfWork)
    : IRequestHandler<UpdateCustomerProfileCommand, Result>
{
    public async Task<Result> Handle(UpdateCustomerProfileCommand cmd, CancellationToken ct)
    {
        var customer = await repository.GetByIdAsync(cmd.CustomerId, ct);
        if (customer is null) return Result.Failure("Customer not found.");

        customer.UpdateProfile(cmd.OwnerName, cmd.CompanyName, cmd.OwnerPhone, cmd.Industry);
        await iCustomersUnitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}