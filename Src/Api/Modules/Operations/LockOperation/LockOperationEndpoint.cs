using System;
using System.Threading;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Operations.Application.Operations.LockOperation;

namespace Api.Modules.Operations.LockOperation;

public static class LockOperationEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/{operationId:guid}/lock", async (
            Guid operationId,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new LockOperationCommand(operationId), ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Conflict(new { error = result.Error });
        }).RequirePermission(PermissionCatalog.OperationsEdit);
    }
}

