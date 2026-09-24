using BuildingBlocks.Application;
using MediatR;

namespace Customers.Application.Queries.GetClientSearchActivity;

public sealed record ClientSearchActivityResponse(
    int SearchesToday,
    int ActiveClients);

public sealed record GetClientSearchActivityQuery
    : IRequest<Result<ClientSearchActivityResponse>>;