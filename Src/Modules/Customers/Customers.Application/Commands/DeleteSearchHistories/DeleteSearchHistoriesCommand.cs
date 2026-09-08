using BuildingBlocks.Application;
using MediatR;

namespace Customers.Application.Commands.DeleteSearchHistories;

public sealed record DeleteSearchHistoriesCommand(
    IReadOnlyCollection<Guid> SearchHistoryIds)
    : IRequest<Result<DeleteSearchHistoriesResponse>>;

public sealed record DeleteSearchHistoriesResponse(
    int DeletedCount);