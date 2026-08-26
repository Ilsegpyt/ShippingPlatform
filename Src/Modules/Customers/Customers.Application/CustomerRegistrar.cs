using Customers.Application.Abstractions;
using Customers.Application.Contracts;
using Customers.Domain;

namespace Customers.Application;

internal sealed class CustomerRegistrar(ICustomerRepository repository, ICustomersUnitOfWork iCustomersUnitOfWork) : ICustomerRegistrar
{
    public async Task<Guid> RegisterAsync(string ownerName, string companyName, string ownerPhone, string ownerEmail, string? industry, Guid ownerUserId, CancellationToken ct)
    {
        var customer = Customer.Register(ownerName, companyName, ownerPhone, ownerEmail, industry, ownerUserId);
        await repository.AddAsync(customer, ct);
        await iCustomersUnitOfWork.SaveChangesAsync(ct);
        return customer.Id;
    }

}