using BuildingBlocks.Application;
using CustomsClearance.Application.Abstractions.Repositories;
using CustomsClearance.Domain.Entities;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.GetCustomsShipmentById;

public sealed class GetCustomsShipmentByIdQueryHandler(
    ICustomsShipmentRepository customsShipmentRepository)
    : IRequestHandler<GetCustomsShipmentByIdQuery, Result<CustomsShipment>>
{
    public async Task<Result<CustomsShipment>> Handle(
        GetCustomsShipmentByIdQuery query,
        CancellationToken ct)
    {
        var shipment = await customsShipmentRepository.GetByIdAsync(query.Id, ct);

        if (shipment is null)
            return Result.Failure<CustomsShipment>("Customs shipment was not found.");

        return Result.Success(shipment);
    }
}
