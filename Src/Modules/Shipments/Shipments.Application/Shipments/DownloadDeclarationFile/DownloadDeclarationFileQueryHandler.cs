using MediatR;
using Shipments.Application.Abstractions;
using Shipments.Domain.Declarations;

namespace Shipments.Application.Shipments.DownloadDeclarationFile;

public sealed class DownloadDeclarationFileQueryHandler(
    IDeclarationFileRepository declarationFileRepository,
    IShipmentRepository shipmentRepository)
    : IRequestHandler<
        DownloadDeclarationFileQuery,
        DownloadDeclarationFileResult?>
{
    public async Task<DownloadDeclarationFileResult?> Handle(
        DownloadDeclarationFileQuery request,
        CancellationToken ct)
    {
        var shipment =
            await shipmentRepository.GetByIdAsync(
                request.ShipmentId,
                ct);

        if (shipment is null)
            return null;

        if (request.CustomerId.HasValue &&
            shipment.CustomerId != request.CustomerId.Value)
        {
            return null;
        }

        var files =
            await declarationFileRepository.GetByShipmentIdAsync(
                request.ShipmentId,
                ct);

        var file = files.FirstOrDefault(x =>
            x.Id == request.DeclarationFileId);

        if (file is null)
            return null;

        return new DownloadDeclarationFileResult(
            file.FileName,
            file.StorageKey);
    }
}