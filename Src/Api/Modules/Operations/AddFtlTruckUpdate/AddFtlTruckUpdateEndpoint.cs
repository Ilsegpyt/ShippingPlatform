using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.AddFtlTruckUpdate;

namespace Api.Modules.Operations.AddFtlTruckUpdate;

public static class AddFtlTruckUpdateEndpoint
{
    public sealed record AddTruckUpdateRequest(string? TripId, string Note);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/ftl/{reference}/truck-updates", HandleAddUpdate)
            .RequirePermission(PermissionCatalog.OperationsEdit);

        // Support front route
        app.MapPost("/api/v1/ftl/operations/{reference}/truck-updates", HandleAddUpdate)
            .RequirePermission(PermissionCatalog.OperationsEdit);
    }

    private static async Task<IResult> HandleAddUpdate(
        string reference,
        [FromBody] AddTruckUpdateRequest body,
        ISender sender,
        CancellationToken ct)
    {
        var command = new AddFtlTruckUpdateCommand(reference, body.TripId, body.Note);
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}

