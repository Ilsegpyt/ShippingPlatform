using BuildingBlocks.Application;
using MediatR;

namespace Shipments.Application.Shipments.UploadDeclarationFile;

public sealed record UploadDeclarationFileCommand(
    Guid ShipmentId,
    Guid CustomerId,
    string FileName,
    Stream Content)
    : IRequest<Result<UploadDeclarationFileResponse>>;

public sealed record UploadDeclarationFileResponse(
    Guid DeclarationFileId,
    string FileName);