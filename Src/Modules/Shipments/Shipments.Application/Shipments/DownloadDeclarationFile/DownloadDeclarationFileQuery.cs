using MediatR;

namespace Shipments.Application.Shipments.DownloadDeclarationFile;

public sealed record DownloadDeclarationFileQuery(
    Guid ShipmentId,
    Guid DeclarationFileId,
    Guid? CustomerId)
    : IRequest<DownloadDeclarationFileResult?>;


public sealed record DownloadDeclarationFileResult(
    string FileName,
    string StorageKey);