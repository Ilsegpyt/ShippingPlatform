using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.Ftl.GetFtlMaster;

namespace Api.Modules.Operations.GetFtlMaster;

public static class GetFtlMasterEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/ftl/master", HandleGetMaster)
            .RequirePermission(PermissionCatalog.OperationsView);

        // Support front routes
        app.MapGet("/api/v1/ftl/master", HandleGetMaster)
            .RequirePermission(PermissionCatalog.OperationsView);
        app.MapGet("/master", HandleGetMaster)
            .RequirePermission(PermissionCatalog.OperationsView);
    }

    private static async Task<IResult> HandleGetMaster(ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetFtlMasterQuery(), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}

