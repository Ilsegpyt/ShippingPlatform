using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.Ftl.GetAllFtlOperations;

namespace Api.Modules.Operations.GetAllFtlOperations;

public static class GetAllFtlOperationsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/ftl", HandleGetAll)
            .RequirePermission(PermissionCatalog.OperationsView);

        // Support front routes
        app.MapGet("/api/v1/ftl/operations", HandleGetAll)
            .RequirePermission(PermissionCatalog.OperationsView);
        app.MapGet("/api/v1/ftl/workspace", HandleGetAll)
            .RequirePermission(PermissionCatalog.OperationsView);
        app.MapGet("/load", HandleGetAll)
            .RequirePermission(PermissionCatalog.OperationsView);
    }

    private static async Task<IResult> HandleGetAll(ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetAllFtlOperationsQuery(), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}
