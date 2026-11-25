using BuildingBlocks.Application;
using CustomsClearance.Domain.Entities;
using CustomsClearance.Domain.Enums;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.GetCustomsShipments;

// Status = Open for the live table, Closed for the archive.
public sealed record GetCustomsShipmentsQuery(
    ReportType ReportType,
    CustomsShipmentStatus Status,
    PaginationRequest Pagination)
    : IRequest<Result<PagedResult<CustomsShipment>>>;
