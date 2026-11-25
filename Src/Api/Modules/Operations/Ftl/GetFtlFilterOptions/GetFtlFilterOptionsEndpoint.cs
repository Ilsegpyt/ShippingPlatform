using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.Ftl.GetFtlFilterOptions;

namespace Api.Modules.Operations.Ftl.GetFtlFilterOptions;

public static class GetFtlFilterOptionsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/ftl/filter-options", HandleRequest)
            .RequirePermission(PermissionCatalog.FtlOperationsView);

        app.MapGet("/api/v1/ftl/filter-options", HandleRequest)
            .RequirePermission(PermissionCatalog.FtlOperationsView);
    }

    private static async Task<IResult> HandleRequest(
        string? operationType,
        string? status,
        bool? onlyActive,
        bool? isDeleted,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetFtlFilterOptionsQuery(
            OperationType: operationType,
            Status: status,
            OnlyActive: onlyActive ?? false,
            IsDeleted: isDeleted ?? false), ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}
