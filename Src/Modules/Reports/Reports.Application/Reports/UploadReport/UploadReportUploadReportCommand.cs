using BuildingBlocks.Application;
using MediatR;
using Reports.Domain.Report;

namespace Reports.Application.Reports.UploadReport;

public sealed record UploadReportCommand(
    Guid CustomerId,
    string? ShipmentRef,
    ReportCategory Category,
    ReportService Service,
    ReportShipmentType ShipmentType,
    string FileName,
    Stream File,
    Guid UploadedByUserId)
    : IRequest<Result<Guid>>;