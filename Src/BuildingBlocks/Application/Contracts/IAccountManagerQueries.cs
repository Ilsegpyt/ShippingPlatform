namespace BuildingBlocks.Application.Contracts;

public interface IAccountManagerQueries
{
    Task<bool> IsAssignedToCustomerAsync(
        Guid accountManagerId,
        Guid customerId,
        CancellationToken ct);
}