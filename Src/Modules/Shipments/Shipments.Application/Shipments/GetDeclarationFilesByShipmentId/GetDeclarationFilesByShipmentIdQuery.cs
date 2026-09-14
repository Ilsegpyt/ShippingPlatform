using BuildingBlocks.Application;
using MediatR;
using Shipments.Application.Abstractions;

namespace Shipments.Application.Shipments.GetDeclarationFilesByShipmentId;

public sealed record GetDeclarationFilesByShipmentIdQuery(
    Guid ShipmentId,
    Guid? CustomerId)
    : IRequest<Result<IReadOnlyList<DeclarationFileResponse>>>;

public sealed record DeclarationFileResponse(
    Guid Id,
    string FileName,
    DateTime UploadedAtUtc);

public sealed class GetDeclarationFilesByShipmentIdQueryHandler(
    IShipmentRepository shipmentRepository,
    IDeclarationFileRepository declarationFileRepository)
    : IRequestHandler<
        GetDeclarationFilesByShipmentIdQuery,
        Result<IReadOnlyList<DeclarationFileResponse>>>
{
    public async Task<Result<IReadOnlyList<DeclarationFileResponse>>> Handle(
        GetDeclarationFilesByShipmentIdQuery query,
        CancellationToken ct)
    {
        var shipment = await shipmentRepository.GetByIdAsync(
            query.ShipmentId,
            ct);

        if (shipment is null)
        {
            return Result.Failure<IReadOnlyList<DeclarationFileResponse>>(
                "Shipment was not found.");
        }

        if (query.CustomerId.HasValue &&
         shipment.CustomerId != query.CustomerId.Value)
        {
            return Result.Failure<IReadOnlyList<DeclarationFileResponse>>(
                "Shipment was not found.");
        }

        var files = await declarationFileRepository.GetByShipmentIdAsync(
            query.ShipmentId,
            ct);

        var response = files
            .Select(file => new DeclarationFileResponse(
                file.Id,
                file.FileName,
                file.UploadedAtUtc))
            .ToList();

        return Result.Success<IReadOnlyList<DeclarationFileResponse>>(
            response);
    }
}