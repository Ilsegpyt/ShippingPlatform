namespace Identity.Contracts;

public interface IAccountManagerQueries
{
    Task<bool> IsAssignedToCustomerAsync(
        Guid accountManagerId,
        Guid customerId,
        CancellationToken ct);


    Task<IReadOnlyList<Guid>> GetAssignedCustomerIdsAsync(
     Guid accountManagerId,
     CancellationToken ct);
}