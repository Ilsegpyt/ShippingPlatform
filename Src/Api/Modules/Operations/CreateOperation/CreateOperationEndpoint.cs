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
                request.ShipmentNumber,
                request.CertificateNumber,
                request.InvoicesReceivedDate);

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
    string? ShipmentNumber,
    string? CertificateNumber,
    DateTime? InvoicesReceivedDate);