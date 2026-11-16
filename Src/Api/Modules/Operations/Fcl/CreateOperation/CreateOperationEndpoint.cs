
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.CreateOperation;
using Operations.Domain.Enums;

namespace Api.Modules.Operations.CreateOperation;

public static class CreateOperationEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations", async (
            CreateOperationRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var isExport = request.OperationType == OperationType.Export;
            var command = new CreateOperationCommand(
                // Common
                request.ClientId,
                request.ShippingLineId,
                request.OperationType,
                request.NumberOfContainers,

                // Import
                request.MBLNumber,
                isExport ? null : request.Destination,
                request.PODId,
                request.FreeTimeTill,
                request.RequiredOffloadingDate,
                request.RequiredOffloadingTime,

                // Export
                request.ClearanceType,
                request.BookingNumber,
                request.POLId,
                request.POWId,
                request.CutOffDate,
                request.ExportDestination // الإضافة المطلوبة
            );

            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);
    }
}

public sealed record CreateOperationRequest(
    Guid ClientId,
    Guid ShippingLineId,
    OperationType OperationType,
    int NumberOfContainers,

    // Import
    string? MBLNumber,
    string? Destination,
    Guid? PODId,
    DateOnly? FreeTimeTill,
    DateOnly? RequiredOffloadingDate,
    TimeOnly? RequiredOffloadingTime,

    // Export
    ClearanceType? ClearanceType,
    string? BookingNumber,
    Guid? POLId,
    Guid? POWId,
    DateOnly? CutOffDate,
    string? ExportDestination = null
);
