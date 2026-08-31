using BuildingBlocks.Application.Contracts;
using Customers.Application.Abstractions;
using Customers.Domain;

namespace Customers.Application;

/// <summary>
/// Application service responsible for Customer queries exposed
/// through the shared ICustomerQueries contract.
/// </summary>
internal sealed class CustomerQueries(ICustomerRepository repository)
    : ICustomerQueries
{
    public async Task<bool> IsOwnerAsync(
        Guid customerId,
        Guid userId,
        CancellationToken ct)
    {
        var customer = await repository.GetByIdAsync(customerId, ct);

        return customer?.IsOwnedBy(userId) ?? false;
    }

    public async Task<CustomerAuthInfo?> GetByOwnerUserIdAsync(
        Guid userId,
        CancellationToken ct)
    {
        var customer = await repository.GetByOwnerUserIdAsync(userId, ct);

        return customer is null
            ? null
            : new CustomerAuthInfo(
                customer.Id,
                customer.Status == CustomerStatus.Active);
    }

    public async Task<bool> ExistsAsync(
        Guid customerId,
        CancellationToken ct)
    {
        return await repository.GetByIdAsync(customerId, ct) is not null;
    }
    public async Task<CustomerAssignmentInfo?> GetForAssignmentAsync(
    Guid customerId,
    CancellationToken ct)
    {
        var customer = await repository.GetByIdAsync(customerId, ct);

        return customer is null
            ? null
            : new CustomerAssignmentInfo(
                customer.Id,
                customer.Status == CustomerStatus.Active);
    }
}