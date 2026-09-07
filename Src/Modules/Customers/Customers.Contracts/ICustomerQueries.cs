namespace Customers.Contracts;

// Defines the Customer queries that can be used by other modules
public interface ICustomerQueries
{
    // Gets the Customer linked to a specific User.
    Task<CustomerAuthInfo?> GetByUserIdAsync(Guid userId, CancellationToken ct);

    // Gets a Customer by its ID.
    Task<CustomerInfo?> GetByIdAsync(Guid customerId, CancellationToken ct);
}

// Customer information needed by Identity.
public sealed record CustomerAuthInfo(Guid CustomerId, bool IsActive);

// Basic Customer information exposed to other modules.
public sealed record CustomerInfo(Guid CustomerId, bool IsActive);
