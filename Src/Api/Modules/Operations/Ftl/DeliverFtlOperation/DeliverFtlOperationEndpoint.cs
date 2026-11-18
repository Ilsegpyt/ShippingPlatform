using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.Ftl.DeliverFtlOperation;

namespace Api.Modules.Operations.Ftl.DeliverFtlOperation;

public static class DeliverFtlOperationEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/ftl/{reference}/deliver", HandleDeliver)
            .RequirePermission(PermissionCatalog.FtlOperationsDeliver);

        app.MapPut("/api/v1/ftl/operations/{reference}/deliver", HandleDeliver)
            .RequirePermission(PermissionCatalog.FtlOperationsDeliver);

        app.MapPost("/api/operations/ftl/{reference}/deliver", HandleDeliver)
            .RequirePermission(PermissionCatalog.FtlOperationsDeliver);
    }

    private static async Task<IResult> HandleDeliver(
        string reference,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new DeliverFtlOperationCommand(reference), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true })
            : Results.BadRequest(new { error = result.Error });
    }
}
