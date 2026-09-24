using BuildingBlocks.Application;
using MediatR;
using Shipments.Application.Abstractions;
using Shipments.Domain.Shipments;

namespace Shipments.Application.Shipments.GetShipmentExceptions;

public sealed class GetShipmentExceptionsQueryHandler(
    IShipmentRepository shipmentRepository)
    : IRequestHandler<
        GetShipmentExceptionsQuery,
        Result<GetShipmentExceptionsResponse>>
{
    public async Task<Result<GetShipmentExceptionsResponse>> Handle(
        GetShipmentExceptionsQuery query,
        CancellationToken ct)
    {
        var missingDocuments = await shipmentRepository.CountByStatusAsync(
            ShipmentStatus.MissingDocs,
            ct);

        var cancelled = await shipmentRepository.CountByStatusAsync(
            ShipmentStatus.Cancelled,
            ct);

        var total = missingDocuments + cancelled;

        return new GetShipmentExceptionsResponse(
            total,
            missingDocuments,
            cancelled);
    }
}