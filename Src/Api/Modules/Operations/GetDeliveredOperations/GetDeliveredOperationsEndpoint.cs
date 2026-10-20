using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.GetDeliveredOperations;
using Operations.Domain.Enums;

namespace Api.Modules.Operations.GetDeliveredOperations;

public static class GetDeliveredOperationsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/delivered", async (
            string? type,
            ISender sender,
            CancellationToken ct) =>
        {
            OperationType? operationType = null;

            if (!string.IsNullOrWhiteSpace(type))
            {
                if (!Enum.TryParse<OperationType>(type, ignoreCase: true, out var parsedType) ||
                    !Enum.IsDefined(parsedType))
                {
                    return Results.BadRequest(new
                    {
                        Error = "Invalid operation type. Use Import or Export."
                    });
                }

                operationType = parsedType;
            }

            var result = await sender.Send(
                new GetDeliveredOperationsQuery(operationType),
                ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { Error = result.Error });
        })
        .RequirePermission(PermissionCatalog.OperationsView);
    }
}
