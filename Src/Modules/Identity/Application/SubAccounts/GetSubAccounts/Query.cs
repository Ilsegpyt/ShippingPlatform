using BuildingBlocks.Application;
using Customers.Contracts;
using Identity.Domain.Enums;
using MediatR;

namespace Identity.Application.SubAccounts.GetSubAccounts;

public interface ISubAccountReadRepository
{
    Task<IReadOnlyList<SubAccountListItem>> GetByCustomerIdAsync(
        Guid organizationId,
        CancellationToken ct);
}

public sealed record SubAccountListItem(
    Guid Id,
    string Name,
    string Email,
    string Status,
    IReadOnlyList<string> ScopeDescriptions,
    ScopeType ScopeType,
    IReadOnlyList<SubAccountScopeItem> Scopes);

public sealed record GetSubAccountsQuery(Guid CustomerId)
    : IRequest<Result<IReadOnlyList<SubAccountListItem>>>;
public sealed record SubAccountScopeItem(
    ScopeCategory Category,
    ScopeService Service,
    ScopeShipmentType Type);
public sealed class GetSubAccountsHandler
    : IRequestHandler<GetSubAccountsQuery, Result<IReadOnlyList<SubAccountListItem>>>
{
    private readonly ISubAccountReadRepository _reads;
    private readonly ICustomerQueries _customerQueries;

    public GetSubAccountsHandler(
        ISubAccountReadRepository reads,
        ICustomerQueries customerQueries)
    {
        _reads = reads;
        _customerQueries = customerQueries;
    }

    public async Task<Result<IReadOnlyList<SubAccountListItem>>> Handle(
        GetSubAccountsQuery request,
        CancellationToken ct)
    {
        var customer = await _customerQueries.GetByIdAsync(
            request.CustomerId,
            ct);

        if (customer is null ||
            customer.IsDeleted ||
            !customer.IsActive)
        {
            return Result.Failure<IReadOnlyList<SubAccountListItem>>(
                "Only active customers can view sub-accounts.");
        }

        var items = await _reads.GetByCustomerIdAsync(
            request.CustomerId,
            ct);

        return Result.Success(items);
    }
}
