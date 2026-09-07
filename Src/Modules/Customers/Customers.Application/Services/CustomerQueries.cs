using Customers.Application.Abstractions;
using Customers.Contracts;
using Customers.Domain.Entities;

namespace Customers.Application.Services;

/// <summary>
/// Application service responsible for Customer queries exposed
/// through the shared ICustomerQueries contract.
/// </summary>
internal sealed class CustomerQueries(ICustomerRepository repository) : ICustomerQueries
{
    
    public async Task<CustomerAuthInfo?> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        var customer = await repository.GetByUserIdAsync(userId, ct);

        return customer is null
            ? null
            : new CustomerAuthInfo(
                customer.Id,
                customer.Status == CustomerStatus.Active);
    }
  
    public async Task<CustomerInfo?> GetByIdAsync(Guid customerId, CancellationToken ct)
    {
        var customer = await repository.GetByIdAsync(customerId, ct);

        return customer is null
            ? null
            : new CustomerInfo(
                customer.Id,
                customer.Status == CustomerStatus.Active);
    }
}