using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.ImportFtlTrips;

namespace Api.Modules.Operations.ImportFtlTrips;

public static class ImportFtlTripsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/ftl/import", HandleImport)
            .RequirePermission(PermissionCatalog.OperationsCreate);

        // Support front routes
        app.MapPost("/api/v1/ftl/trips/import", HandleImport)
            .RequirePermission(PermissionCatalog.OperationsCreate);
        app.MapPost("/importTrips", HandleImport)
            .RequirePermission(PermissionCatalog.OperationsCreate);
    }

    private static async Task<IResult> HandleImport(
        [FromBody] ImportFtlTripsCommand command,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}

