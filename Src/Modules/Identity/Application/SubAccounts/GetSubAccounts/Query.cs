using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.SubAccounts.GetSubAccounts;

public interface ISubAccountReadRepository
{
    Task<IReadOnlyList<SubAccountListItem>> GetByOrganizationIdAsync(
        Guid organizationId,
        CancellationToken ct);
}

public sealed record SubAccountListItem(
    Guid Id,
    string Name,
    string Email,
    string Status,
    IReadOnlyList<string> ScopeDescriptions);

public sealed record GetSubAccountsQuery(Guid OrganizationId)
    : IRequest<Result<IReadOnlyList<SubAccountListItem>>>;

public sealed class GetSubAccountsHandler
    : IRequestHandler<GetSubAccountsQuery, Result<IReadOnlyList<SubAccountListItem>>>
{
    private readonly ISubAccountReadRepository _reads;

    public GetSubAccountsHandler(ISubAccountReadRepository reads)
        => _reads = reads;

    public async Task<Result<IReadOnlyList<SubAccountListItem>>> Handle(
        GetSubAccountsQuery request,
        CancellationToken ct)
    {
        var items = await _reads.GetByOrganizationIdAsync(
            request.OrganizationId,
            ct);

        return Result.Success(items);
    }
}
