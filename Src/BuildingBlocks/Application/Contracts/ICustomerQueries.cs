namespace BuildingBlocks.Application.Contracts;

/// <summary>
/// Public contract implemented by the Customers module and consumed by
/// other modules that need customer-related queries without referencing
/// Customers.Application or Customers.Domain directly.
/// </summary>
public interface ICustomerQueries
{
    /// <summary>
    /// Checks whether a given user is the Owner of a specific Customer.
    /// </summary>
    Task<bool> IsOwnerAsync(
        Guid customerId,
        Guid userId,
        CancellationToken ct);

    /// <summary>
    /// Gets the Customer owned by the specified user.
    /// Used by Identity to build authentication-related information.
    /// </summary>
    Task<CustomerAuthInfo?> GetByOwnerUserIdAsync(
        Guid userId,
        CancellationToken ct);

    /// <summary>
    /// Checks whether a Customer exists.
    /// </summary>
    Task<bool> ExistsAsync(
        Guid customerId,
        CancellationToken ct);

    Task<CustomerAssignmentInfo?> GetForAssignmentAsync(
    Guid customerId,
    CancellationToken ct);
}

/// <summary>
/// Customer information exposed to other modules.
/// Customers.Domain types are deliberately not exposed here.
/// </summary>
public sealed record CustomerAuthInfo(
    Guid CustomerId,
    bool IsActive);

public sealed record CustomerAssignmentInfo(
    Guid CustomerId,
    bool IsActive);