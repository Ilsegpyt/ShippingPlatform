using CustomsClearance.Domain.Enums;

namespace CustomsClearance.Application.CustomsShipments.Inactive;

// Which date the Inactive archive range filter applies to.
public enum CustomsInactiveDateType
{
    Inactive, // DeletedAtUtc (falls back to CreatedAtUtc)
    Created,  // CreatedAtUtc
}

// Filters of the paged Inactive archive. ReportType null = all reports.
// FromUtc is inclusive, ToUtcExclusive exclusive (Cairo calendar days turned into UTC instants).
public sealed record CustomsInactiveFilter(
    ReportType? ReportType,
    string? Search,
    string? InactiveBy,
    CustomsInactiveDateType DateType,
    DateTime? FromUtc,
    DateTime? ToUtcExclusive);

// One row of the grouped counts query.
public sealed record CustomsShipmentCountRow(
    ReportType ReportType,
    CustomsShipmentStatus Status,
    bool IsDeleted,
    int Count);
