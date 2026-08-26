using System.Transactions;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Contracts;
using Customers.Application.Abstractions;
using Customers.Domain;
using MediatR;

namespace Customers.Application.Customers.RegisterCustomer;



public sealed class RegisterCustomerCommandHandler(ICustomerRepository customerRepository, ICustomersUnitOfWork iCustomersUnitOfWork, IIdentityUserRegistrar identityRegistrar)
    : IRequestHandler<RegisterCustomerCommand, Result<RegisterCustomerResponse>>
{
    public async Task<Result<RegisterCustomerResponse>> Handle(RegisterCustomerCommand cmd, CancellationToken ct)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var userId = await identityRegistrar.CreateUserAsync( cmd.OwnerEmail, ct);

        var customer = Customer.Register(cmd.OwnerName, cmd.CompanyName, cmd.OwnerPhone, cmd.OwnerEmail, cmd.Industry, userId);

        await customerRepository.AddAsync(customer, ct);
        await iCustomersUnitOfWork.SaveChangesAsync(ct);

        scope.Complete();

        return Result.Success(new RegisterCustomerResponse(customer.Id, identityRegistrar.GetDefaultPassword()));
    }
}