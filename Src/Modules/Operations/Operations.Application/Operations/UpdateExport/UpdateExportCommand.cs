
using BuildingBlocks.Application;
using MediatR;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.UpdateExport;

public sealed record UpdateExportCommand(
    Guid OperationId,
    string? CertificateNumber,
    string? Customs,
    IReadOnlyCollection<UpdateExportContainerItem> Containers
) : IRequest<Result<bool>>;

public sealed record UpdateExportContainerItem(
    Guid ContainerId,
    ContainerType? ContainerType,
    string? ContainerNumber,
    DateOnly? RequiredLoadingDate,
    TimeOnly? RequiredLoadingTime,
    string? Destination
);
