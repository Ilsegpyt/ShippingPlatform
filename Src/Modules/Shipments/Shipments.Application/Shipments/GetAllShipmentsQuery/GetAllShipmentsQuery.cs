using BuildingBlocks.Application;
using MediatR;
using Shipments.Application.Abstractions;
using Shipments.Domain.Shipments;

namespace Shipments.Application.Shipments.GetAllShipmentsQuery;

public sealed record GetAllShipmentsQuery(PaginationRequest Pagination)
    : IRequest<PagedResult<ShipmentResponse>>;

public sealed record ShipmentResponse(
    Guid Id,
    string ShipmentRef,
    Guid CustomerId,
    Guid ScheduleId,
    string Mode,
    string Carrier,
    string ContainerType,
    int Quantity,
    decimal Rate,
    decimal Total,
    ShipmentStatus Status,
    string? MBL,
    string? HBL,
    string? MAWB,
    string? BookingConfirmationNumber,
    DateTime CreatedAtUtc);

public sealed class GetAllShipmentsQueryHandler(
    IShipmentRepository repository)
    : IRequestHandler<GetAllShipmentsQuery, PagedResult<ShipmentResponse>>
{
    public async Task<PagedResult<ShipmentResponse>> Handle(
        GetAllShipmentsQuery query,
        CancellationToken ct)
    {
        var page = query.Pagination.PageNumber;
        var pageSize = query.Pagination.PageSize;
        var skip = (page - 1) * pageSize;

        var totalCount = await repository.CountAsync(ct);

        var shipments = await repository.GetAllAsync(skip, pageSize, ct);

        var items = shipments
            .Select(x => new ShipmentResponse(
                x.Id,
                x.ShipmentRef,
                x.CustomerId,
                x.ScheduleId,
                x.Mode,
                x.Carrier,
                x.ContainerType,
                x.Quantity,
                x.Rate,
                x.Total,
                x.Status,
                x.MBL,
                x.HBL,
                x.MAWB,
                x.BookingConfirmationNumber,
                x.CreatedAtUtc))
            .ToList();

        return new PagedResult<ShipmentResponse>(
            items,
            totalCount,
            page,
            pageSize);
    }
}