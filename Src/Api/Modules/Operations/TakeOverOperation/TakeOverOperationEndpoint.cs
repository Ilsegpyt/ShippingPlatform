using System;
using System.Threading;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Operations.Application.Operations.TakeOverOperation;

namespace Api.Modules.Operations.TakeOverOperation;

public static class TakeOverOperationEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/{operationId:guid}/take-over", async (
            Guid operationId,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new TakeOverOperationCommand(operationId), ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { error = result.Error });
        }).RequirePermission(PermissionCatalog.OperationsReopen);
    }
}

