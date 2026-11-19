using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.Ftl.GetAllFtlOperations;
using Operations.Application.Operations.Ftl.GetPagedFtlOperations;

namespace Api.Modules.Operations.Ftl.GetAllFtlOperations;

public static class GetAllFtlOperationsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/ftl", HandleRequest)
            .RequirePermission(PermissionCatalog.FtlOperationsView);

        // Support front routes
        app.MapGet("/api/v1/ftl/operations", HandleRequest)
            .RequirePermission(PermissionCatalog.FtlOperationsView);
        app.MapGet("/api/v1/ftl/workspace", HandleRequest)
            .RequirePermission(PermissionCatalog.FtlOperationsView);
        app.MapGet("/load", HandleRequest)
            .RequirePermission(PermissionCatalog.FtlOperationsView);
    }

    private static async Task<IResult> HandleRequest(
        int? page,
        int? pageSize,
        string? operationType,
        string? status,
        bool? onlyActive,
        bool? isDeleted,
        string? search,
        string? client,
        string? transporter,
        string? terminal,
        string? createdBy,
        string? dateType,
        string? dateFrom,
        string? dateTo,
        string? sortBy,
        string? sortDirection,
        ISender sender,
        CancellationToken ct)
    {
        if (page.HasValue)
        {
            var pagedResult = await sender.Send(new GetPagedFtlOperationsQuery(
                Page: page.Value,
                PageSize: pageSize ?? 20,
                OperationType: operationType,
                Status: status,
                OnlyActive: onlyActive ?? false,
                IsDeleted: isDeleted ?? false,
                Search: search,
                Client: client,
                Transporter: transporter,
                Terminal: terminal,
                CreatedBy: createdBy,
                DateType: dateType,
                DateFrom: dateFrom,
                DateTo: dateTo,
                SortBy: sortBy,
                SortDirection: sortDirection ?? "desc"), ct);

            return pagedResult.IsSuccess
                ? Results.Ok(pagedResult.Value)
                : Results.BadRequest(new { error = pagedResult.Error });
        }

        var result = await sender.Send(new GetAllFtlOperationsQuery(), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}
