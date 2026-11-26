using BuildingBlocks.Application;
using CustomsClearance.Application.CustomsShipments.Alerts;
using CustomsClearance.Domain.Entities;
using CustomsClearance.Domain.Enums;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.GetCustomsShipments;

// Status = Open for the live table, Closed for the archive.
// Alert filters only Open Ocean Import shipments; it is ignored for other reports and the archive.
public sealed record GetCustomsShipmentsQuery(
    ReportType ReportType,
    CustomsShipmentStatus Status,
    PaginationRequest Pagination,
    CustomsAlertFilter? Alert = null)
    : IRequest<Result<PagedResult<CustomsShipmentListItem>>>;

// Alerts is null for shipments that carry no alerts (non Ocean Import, or Closed).
public sealed record CustomsShipmentListItem(
    CustomsShipment Shipment,
    CustomsShipmentAlerts? Alerts);
