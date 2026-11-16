using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.DeleteOperation;

namespace Api.Modules.Operations.DeleteOperation;

public static class DeleteOperationEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/operations/{operationId:guid}", async (
            Guid operationId,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new DeleteOperationCommand(operationId), ct);

            return result.IsSuccess
                ? Results.Ok(new { success = true })
                : Results.BadRequest(result.Error);
        }).RequirePermission(PermissionCatalog.OperationsDelete);
    }
}
