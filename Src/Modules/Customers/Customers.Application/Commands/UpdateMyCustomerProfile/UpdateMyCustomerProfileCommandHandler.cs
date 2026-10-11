using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using Customers.Domain.Entities;
using MediatR;

namespace Customers.Application.Commands.UpdateMyCustomerProfile;

public sealed class UpdateMyCustomerProfileCommandHandler(
    ICustomerRepository repository,
    ICustomersUnitOfWork unitOfWork
) : IRequestHandler<UpdateMyCustomerProfileCommand, Result>
{
    public async Task<Result> Handle(
        UpdateMyCustomerProfileCommand cmd,
        CancellationToken ct)
    {
        var customer = await repository.GetByIdAsync(
            cmd.CustomerId,
            ct);

        if (customer is null)
            return Result.Failure("Customer not found.");

        if (customer.IsDeleted)
            return Result.Failure("Deleted customers cannot update their profile.");

        if (customer.Status != CustomerStatus.Active)
            return Result.Failure("Inactive customers cannot update their profile.");

        customer.UpdateContactInfo(
            cmd.OwnerName,
            cmd.OwnerPhone);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
