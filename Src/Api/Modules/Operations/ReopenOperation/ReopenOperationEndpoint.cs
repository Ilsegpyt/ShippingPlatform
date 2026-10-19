using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.ReopenOperation;

namespace Api.Modules.Operations.ReopenOperation;

public static class ReopenOperationEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/{operationId:guid}/reopen", async (
            Guid operationId,
            ReopenOperationRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new ReopenOperationCommand(operationId, request.Reason), ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        }).RequirePermission(PermissionCatalog.OperationsReopen);
    }
}

public sealed record ReopenOperationRequest(string Reason);
