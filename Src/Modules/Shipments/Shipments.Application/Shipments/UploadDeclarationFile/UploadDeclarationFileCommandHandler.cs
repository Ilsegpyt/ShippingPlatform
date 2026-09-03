using BuildingBlocks.Application;
using MediatR;
using Shipments.Application.Abstractions;
using Shipments.Domain.Declarations;

namespace Shipments.Application.Shipments.UploadDeclarationFile;

public sealed class UploadDeclarationFileCommandHandler(
    IFileStorage fileStorage,
    IShipmentRepository shipmentRepository,
    IDeclarationFileRepository declarationFileRepository,
    IShipmentsUnitOfWork unitOfWork)
    : IRequestHandler<
        UploadDeclarationFileCommand,
        Result<UploadDeclarationFileResponse>>
{
    public async Task<Result<UploadDeclarationFileResponse>> Handle(
        UploadDeclarationFileCommand command,
        CancellationToken ct)
    {
        var shipment = await shipmentRepository.GetByIdAsync(
            command.ShipmentId,
            ct);

        if (shipment is null)
        {
            return Result.Failure<UploadDeclarationFileResponse>(
                "Shipment was not found.");
        }

        if (shipment.CustomerId != command.CustomerId)
        {
            return Result.Failure<UploadDeclarationFileResponse>(
                "You are not allowed to upload a declaration file for this shipment.");
        }

        var storageKey = await fileStorage.SaveAsync(
            command.Content,
            command.FileName,
            ct);

        var declarationFile = DeclarationFile.Create(
            shipment.Id,
            command.FileName,
            storageKey);

        await declarationFileRepository.AddAsync(
            declarationFile,
            ct);

        await unitOfWork.SaveChangesAsync(ct);

        return new UploadDeclarationFileResponse(
            declarationFile.Id,
            declarationFile.FileName);
    }
}