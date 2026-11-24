using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Operations.Application.Operations.Ftl.TakeOverFtlOperation;

namespace Api.Modules.Operations.Ftl.TakeOverFtlOperation;

public static class TakeOverFtlOperationEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/ftl/{reference}/take-over", HandleTakeOver)
            .RequirePermission(PermissionCatalog.FtlOperationsReopen);

        // Support front route
        app.MapPost("/api/v1/ftl/operations/{reference}/take-over", HandleTakeOver)
            .RequirePermission(PermissionCatalog.FtlOperationsReopen);
    }

    private static async Task<IResult> HandleTakeOver(
        string reference,
        ISender sender,
        CancellationToken ct)
    {
        var command = new TakeOverFtlOperationCommand(reference);
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}
