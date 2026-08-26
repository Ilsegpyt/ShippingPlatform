namespace BuildingBlocks.Application.Contracts;

/// <summary>Public contract implemented by the Customers module and consumed by the
/// Identity module (Login/Refresh flow needs to know whether a login belongs to a
/// Customer's Owner). Lives in BuildingBlocks — neither module may reference the
/// other's Application/Domain/Infrastructure layer directly.</summary>
public interface ICustomerQueries
{
    /// <summary>Used when checking whether a given user is the Owner of a specific
    /// Customer (e.g. before letting them deactivate a SubAccount belonging to
    /// another company).</summary>
    Task<bool> IsOwnerAsync(Guid customerId, Guid userId, CancellationToken ct);

    /// <summary>Used by Identity's Login/Refresh flow to check whether userId is a
    /// Customer's Owner, and to get what's needed to build the JWT claims for them
    /// (the CustomerId for org_id, and whether the Customer is active).</summary>
    Task<CustomerAuthInfo?> GetByOwnerUserIdAsync(Guid userId, CancellationToken ct);
}

/// <summary>Deliberately NOT exposing Customers.Domain's CustomerStatus enum here —
/// Identity shouldn't need to know Customers' internal status representation.</summary>
public sealed record CustomerAuthInfo(Guid CustomerId, bool IsActive);