using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using MediatR;

namespace Customers.Application.Queries.GetClientSearchActivity;

public sealed class GetClientSearchActivityQueryHandler(
    ISearchHistoryRepository repository)
    : IRequestHandler<
        GetClientSearchActivityQuery,
        Result<ClientSearchActivityResponse>>
{
    public async Task<Result<ClientSearchActivityResponse>> Handle(
        GetClientSearchActivityQuery query,
        CancellationToken ct)
    {
        var todayUtc = DateTime.UtcNow.Date;
        var tomorrowUtc = todayUtc.AddDays(1);

        var searchesToday = await repository.CountTodayAsync(
            todayUtc,
            tomorrowUtc,
            ct);

        var activeClients = await repository.CountDistinctCustomersTodayAsync(
            todayUtc,
            tomorrowUtc,
            ct);

        return Result.Success(
            new ClientSearchActivityResponse(
                searchesToday,
                activeClients));
    }
}