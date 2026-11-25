using BuildingBlocks.Application;
using CustomsClearance.Application.Abstractions.Repositories;
using CustomsClearance.Domain.Entities;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.GetCustomsUpdateLogs;

public sealed class GetCustomsUpdateLogsQueryHandler(
    ICustomsShipmentRepository customsShipmentRepository)
    : IRequestHandler<
        GetCustomsUpdateLogsQuery,
        Result<IReadOnlyList<CustomsUpdateLog>>>
{
    public async Task<Result<IReadOnlyList<CustomsUpdateLog>>> Handle(
        GetCustomsUpdateLogsQuery query,
        CancellationToken ct)
    {
        if (!await customsShipmentRepository.ExistsAsync(query.ShipmentId, ct))
            return Result.Failure<IReadOnlyList<CustomsUpdateLog>>("Customs shipment was not found.");

        var logs = await customsShipmentRepository.GetUpdateLogsAsync(query.ShipmentId, ct);

        return Result.Success(logs);
    }
}
