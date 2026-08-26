using BuildingBlocks.Application.Contracts;
using Customers.Application.Abstractions;
using Customers.Domain;

// Application Service realted to Customer even if his interface is in BuildingBlock
namespace Customers.Application;

internal sealed class CustomerQueries(ICustomerRepository repository) : ICustomerQueries
{
    public async Task<bool> IsOwnerAsync(Guid customerId, Guid userId, CancellationToken ct)
    {
        var customer = await repository.GetByIdAsync(customerId, ct);
        return customer?.IsOwnedBy(userId) ?? false;
    }

    public async Task<CustomerAuthInfo?> GetByOwnerUserIdAsync(Guid userId, CancellationToken ct)
    {
        var customer = await repository.GetByOwnerUserIdAsync(userId, ct);

        return customer is null
            ? null
            : new CustomerAuthInfo(
                customer.Id,
                customer.Status == CustomerStatus.Active);
    }
}
