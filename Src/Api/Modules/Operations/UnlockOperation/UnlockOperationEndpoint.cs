using System;
using System.Threading;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Operations.Application.Operations.UnlockOperation;

namespace Api.Modules.Operations.UnlockOperation;

public static class UnlockOperationEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/{operationId:guid}/unlock", async (
            Guid operationId,
            UnlockOperationRequest? request,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new UnlockOperationCommand(operationId, request?.Force ?? false), ct);

            return result.IsSuccess
                ? Results.Ok(new { success = true })
                : Results.BadRequest(new { error = result.Error });
        }).RequirePermission(PermissionCatalog.OperationsEdit);
    }
}

public sealed record UnlockOperationRequest(bool Force = false);

