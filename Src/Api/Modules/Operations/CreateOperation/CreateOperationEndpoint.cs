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
            var command = new CreateOperationCommand(
                request.ClientId,
                request.ShippingLineId,
                request.OperationType,

                // Import
                request.MBLNumber,
                request.Destination,
                request.PODId,
                request.FreeTimeTill,
                request.RequiredOffloadingDate,
                request.RequiredOffloadingTime,

                // Export
                request.ClearanceType,
                request.BookingNumber,
                request.POLId,
                request.POWId,
                request.CutOffDate);

            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });
    }
}

public sealed record CreateOperationRequest(
    Guid ClientId,
    Guid ShippingLineId,
    OperationType OperationType,

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
    DateOnly? CutOffDate);