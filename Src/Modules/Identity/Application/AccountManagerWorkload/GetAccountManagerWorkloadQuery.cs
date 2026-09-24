using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.AccountManagerWorkload;

public sealed record AccountManagerWorkloadItem(
    Guid AccountManagerId,
    string AccountManagerName,
    int AssignedCustomers,
    int ActiveShipments);

public sealed record GetAccountManagerWorkloadResponse(
    IReadOnlyList<AccountManagerWorkloadItem> Items);

public sealed record GetAccountManagerWorkloadQuery
    : IRequest<Result<GetAccountManagerWorkloadResponse>>;