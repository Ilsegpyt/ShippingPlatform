using System.Transactions;
using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using Customers.Application.Customers.RegisterCustomer;
using Customers.Domain.Entities;
using Identity.Contracts;
using MediatR;

namespace Customers.Application.Commands.RegisterCustomer;



public sealed class RegisterCustomerCommandHandler(ICustomerRepository customerRepository, ICustomersUnitOfWork iCustomersUnitOfWork, IIdentityUserRegistrar identityRegistrar)
    : IRequestHandler<RegisterCustomerCommand, Result<RegisterCustomerResponse>>
{
    public async Task<Result<RegisterCustomerResponse>> Handle(RegisterCustomerCommand cmd, CancellationToken ct)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var userId = await identityRegistrar.CreateUserAsync( cmd.OwnerEmail, ct);

        var customer = Customer.Register(cmd.OwnerName, cmd.CompanyName, cmd.OwnerPhone, cmd.OwnerEmail, cmd.Industry, userId);

        customerRepository.Add(customer );
        await iCustomersUnitOfWork.SaveChangesAsync(ct);

        scope.Complete();

        return Result.Success(new RegisterCustomerResponse(customer.Id, identityRegistrar.GetDefaultPassword()));
    }
}