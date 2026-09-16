using BuildingBlocks.Application;
using Identity.Contracts;
using MediatR;
using Shipments.Application.Abstractions;
using Shipments.Domain.Shipments;

namespace Shipments.Application.Shipments.GetAllShipmentsQuery;

public sealed record GetAllShipmentsQuery(
    PaginationRequest Pagination,
    Guid UserId)
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
    IShipmentRepository repository,
    IAccountManagerQueries accountManagerQueries,
    IUserAccessQueries userAccessQueries)
    : IRequestHandler<GetAllShipmentsQuery, PagedResult<ShipmentResponse>>
{
    public async Task<PagedResult<ShipmentResponse>> Handle(
        GetAllShipmentsQuery query,
        CancellationToken ct)
    {
        var page = query.Pagination.PageNumber;
        var pageSize = query.Pagination.PageSize;
        var skip = (page - 1) * pageSize;

        var userAccess =
            await userAccessQueries.GetAccessInfoAsync(
                query.UserId,
                ct);

        var isAccountManager =
            userAccess is not null &&
            userAccess.IsActive &&
            userAccess.TokenType == "internal" &&
            userAccess.RoleName == "Account Manager";

        int totalCount;
        IReadOnlyList<Shipment> shipments;

        if (isAccountManager)
        {
            var assignedCustomerIds =
                await accountManagerQueries.GetAssignedCustomerIdsAsync(
                    query.UserId,
                    ct);

            if (assignedCustomerIds.Count == 0)
            {
                return new PagedResult<ShipmentResponse>(
                    [],
                    0,
                    page,
                    pageSize);
            }

            totalCount =
                await repository.CountByCustomerIdsAsync(
                    assignedCustomerIds,
                    ct);

            shipments =
                await repository.GetByCustomerIdsAsync(
                    assignedCustomerIds,
                    skip,
                    pageSize,
                    ct);
        }
        else
        {
            totalCount =
                await repository.CountAsync(ct);

            shipments =
                await repository.GetAllAsync(
                    skip,
                    pageSize,
                    ct);
        }

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