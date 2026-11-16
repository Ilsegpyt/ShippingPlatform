using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.GetAllOperations;
using Operations.Application.Operations.GetPagedOperations;

namespace Api.Modules.Operations.GetAllOperations;

public static class GetAllOperationsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations", async (
            bool? onlyActive,
            string? operationType,
            string? status,
            bool? isDeleted,
            string? search,
            string? client,
            string? line,
            string? port,
            string? transporter,
            string? containerType,
            string? destination,
            string? clearanceType,
            string? missingDocs,
            string? createdBy,
            string? customs,
            string? dateType,
            string? dateFrom,
            string? dateTo,
            int? page,
            int? pageSize,
            ISender sender,
            CancellationToken ct) =>
        {
            if (page.HasValue)
            {
                var pagedResult = await sender.Send(new GetPagedOperationsQuery(
                    Page: page.Value,
                    PageSize: pageSize ?? 10,
                    OperationType: operationType,
                    Status: status,
                    OnlyActive: onlyActive ?? false,
                    IsDeleted: isDeleted ?? false,
                    Search: search,
                    Client: client,
                    Line: line,
                    Port: port,
                    Transporter: transporter,
                    ContainerType: containerType,
                    Destination: destination,
                    ClearanceType: clearanceType,
                    MissingDocs: missingDocs,
                    CreatedBy: createdBy,
                    Customs: customs,
                    DateType: dateType,
                    DateFrom: dateFrom,
                    DateTo: dateTo), ct);

                return pagedResult.IsSuccess
                    ? Results.Ok(pagedResult.Value)
                    : Results.BadRequest(new { Error = pagedResult.Error });
            }

            var result = await sender.Send(new GetAllOperationsQuery(onlyActive ?? false), ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { Error = result.Error });
        })
        .RequirePermission(PermissionCatalog.OperationsView);
    }
}
