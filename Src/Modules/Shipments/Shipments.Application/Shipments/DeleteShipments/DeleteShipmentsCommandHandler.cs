using BuildingBlocks.Application;
using MediatR;
using Shipments.Application.Abstractions;

namespace Shipments.Application.Shipments.DeleteShipments;

public sealed class DeleteShipmentsCommandHandler(
    IShipmentRepository shipmentRepository,
    IDeclarationFileRepository declarationFileRepository,
    IFileStorage fileStorage,
    IShipmentsUnitOfWork unitOfWork)
    : IRequestHandler<
        DeleteShipmentsCommand,
        Result<DeleteShipmentsResponse>>
{
    public async Task<Result<DeleteShipmentsResponse>> Handle(
        DeleteShipmentsCommand command,
        CancellationToken ct)
    {
        var shipments = await shipmentRepository.GetByIdsAsync(
            command.ShipmentIds,
            ct);

        if (shipments.Count == 0)
        {
            return Result.Failure<DeleteShipmentsResponse>(
                "No shipments were found.");
        }

        var declarationFiles =
            await declarationFileRepository.GetByShipmentIdsAsync(
                command.ShipmentIds,
                ct);

        foreach (var file in declarationFiles)
        {
            await fileStorage.DeleteAsync(
                file.StorageKey,
                ct);
        }

        shipmentRepository.RemoveRange(shipments);

        await unitOfWork.SaveChangesAsync(ct);

        return new DeleteShipmentsResponse(
            shipments.Count);
    }
}