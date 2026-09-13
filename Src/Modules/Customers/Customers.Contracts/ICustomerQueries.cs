namespace Customers.Contracts;

public interface ICustomerQueries
{
    Task<CustomerAuthInfo?> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<CustomerInfo?> GetByIdAsync(Guid customerId, CancellationToken ct);
    Task<CustomerOwnerInfo?> GetOwnerByCustomerIdAsync(Guid customerId, CancellationToken ct);
    Task<CustomerMeInfo?> GetMeByUserIdAsync(Guid userId, CancellationToken ct);

}

public sealed record CustomerAuthInfo(Guid CustomerId, bool IsActive);
public sealed record CustomerInfo(Guid CustomerId, bool IsActive);
public sealed record CustomerOwnerInfo(Guid CustomerId, Guid OwnerUserId, string OwnerEmail);
public sealed record CustomerMeInfo(Guid CustomerId, string OwnerName, string CompanyName, string OwnerPhone, string OwnerEmail);






