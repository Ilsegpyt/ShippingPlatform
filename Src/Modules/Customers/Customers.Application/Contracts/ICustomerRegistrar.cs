namespace Customers.Application.Contracts;

/// <summary>Public surface exposed to the cross-module registration use case (Api project).
/// No other module may reference Customers.Domain or CustomersDbContext directly.</summary>
public interface ICustomerRegistrar
{
    Task<Guid> RegisterAsync(string ownerName, string companyName, string ownerPhone,
        string ownerEmail, string? industry, Guid ownerUserId, CancellationToken ct);
}