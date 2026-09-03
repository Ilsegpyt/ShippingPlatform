using BuildingBlocks.Application;
using MediatR;
using Shipments.Application.Abstractions;

namespace Shipments.Application.Shipments.DeleteDeclarationFiles;

public sealed class DeleteDeclarationFilesCommandHandler(
    IShipmentRepository shipmentRepository,
    IDeclarationFileRepository declarationFileRepository,
    IFileStorage fileStorage,
    IShipmentsUnitOfWork unitOfWork)
    : IRequestHandler<
        DeleteDeclarationFilesCommand,
        Result<DeleteDeclarationFilesResponse>>
{
    public async Task<Result<DeleteDeclarationFilesResponse>> Handle(
        DeleteDeclarationFilesCommand command,
        CancellationToken ct)
    {
        var shipment = await shipmentRepository.GetByIdAsync(
            command.ShipmentId,
            ct);

        if (shipment is null)
        {
            return Result.Failure<DeleteDeclarationFilesResponse>(
                "Shipment was not found.");
        }

        if (shipment.CustomerId != command.CustomerId)
        {
            return Result.Failure<DeleteDeclarationFilesResponse>(
                "You are not allowed to delete declaration files for this shipment.");
        }

        var declarationFiles =
            await declarationFileRepository.GetByIdsAsync(
                command.ShipmentId,
                command.DeclarationFileIds,
                ct);

        if (declarationFiles.Count == 0)
        {
            return Result.Failure<DeleteDeclarationFilesResponse>(
                "No declaration files were found.");
        }

        foreach (var file in declarationFiles)
        {
            await fileStorage.DeleteAsync(
                file.StorageKey,
                ct);
        }

        declarationFileRepository.RemoveRange(
            declarationFiles);

        await unitOfWork.SaveChangesAsync(ct);

        return new DeleteDeclarationFilesResponse(
            declarationFiles.Count);
    }
}