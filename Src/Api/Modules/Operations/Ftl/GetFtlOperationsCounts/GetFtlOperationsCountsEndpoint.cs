using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.Ftl.GetFtlOperationsCounts;

namespace Api.Modules.Operations.Ftl.GetFtlOperationsCounts;

public static class GetFtlOperationsCountsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/ftl/counts", HandleGetCounts)
            .RequirePermission(PermissionCatalog.FtlOperationsView);

        app.MapGet("/api/v1/ftl/counts", HandleGetCounts)
            .RequirePermission(PermissionCatalog.FtlOperationsView);
    }

    private static async Task<IResult> HandleGetCounts(ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetFtlOperationsCountsQuery(), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}
