
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.UpdateExport;
using Operations.Domain.Enums;

namespace Api.Modules.Operations.UpdateExport;

public static class UpdateExportEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/{operationId:guid}/export", async (
            Guid operationId,
            UpdateExportRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateExportCommand(
                operationId,
                request.ShipmentNumber,
                request.CertificateNumber,
                request.Customs,
                (request.Containers ?? Array.Empty<UpdateExportContainerRequest>())
                    .Select(x => new UpdateExportContainerItem(
                        x.ContainerId,
                        x.ContainerType,
                        x.ContainerNumber,
                        x.RequiredLoadingDate,
                        x.RequiredLoadingTime,
                        x.Destination))
                    .ToArray()
            );

            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok(new { Success = result.Value })
                : Results.BadRequest(new { Error = result.Error });
        }).RequirePermission(PermissionCatalog.OperationsEdit);
    }
}

public sealed record UpdateExportRequest(
    string? ShipmentNumber,
    string? CertificateNumber,
    string? Customs,
    IReadOnlyCollection<UpdateExportContainerRequest> Containers
);

public sealed record UpdateExportContainerRequest(
    Guid ContainerId,
    ContainerType? ContainerType,
    string? ContainerNumber,
    DateOnly? RequiredLoadingDate,
    TimeOnly? RequiredLoadingTime,
    string? Destination
);
