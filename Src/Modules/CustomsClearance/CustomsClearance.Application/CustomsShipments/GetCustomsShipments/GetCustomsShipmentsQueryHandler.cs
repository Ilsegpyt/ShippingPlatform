using BuildingBlocks.Application;
using CustomsClearance.Application.Abstractions.Repositories;
using CustomsClearance.Domain.Entities;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.GetCustomsShipments;

public sealed class GetCustomsShipmentsQueryHandler(
    ICustomsShipmentRepository customsShipmentRepository)
    : IRequestHandler<
        GetCustomsShipmentsQuery,
        Result<PagedResult<CustomsShipment>>>
{
    public async Task<Result<PagedResult<CustomsShipment>>> Handle(
        GetCustomsShipmentsQuery query,
        CancellationToken ct)
    {
        var pageNumber = query.Pagination.PageNumber;
        var pageSize = query.Pagination.PageSize;
        var search = query.Pagination.Search;

        var (items, totalCount) =
            await customsShipmentRepository.GetPagedAsync(
                query.ReportType,
                query.Status,
                pageNumber,
                pageSize,
                search,
                ct);

        var result = new PagedResult<CustomsShipment>(
            items,
            totalCount,
            pageNumber,
            pageSize);

        return Result.Success(result);
    }
}
