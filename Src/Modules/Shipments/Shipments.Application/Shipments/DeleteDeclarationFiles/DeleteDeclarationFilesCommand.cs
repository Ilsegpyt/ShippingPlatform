using BuildingBlocks.Application;
using MediatR;

namespace Shipments.Application.Shipments.DeleteDeclarationFiles;

public sealed record DeleteDeclarationFilesCommand(
    Guid ShipmentId,
    Guid CustomerId,
    IReadOnlyCollection<Guid> DeclarationFileIds)
    : IRequest<Result<DeleteDeclarationFilesResponse>>;

public sealed record DeleteDeclarationFilesResponse(
    int DeletedCount);