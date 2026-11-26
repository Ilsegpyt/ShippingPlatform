using System.Linq.Expressions;
using CustomsClearance.Application.CustomsShipments.Inactive;
using CustomsClearance.Domain.Entities;
using CustomsClearance.Domain.Enums;

namespace CustomsClearance.Application.Abstractions.Repositories;

public interface ICustomsShipmentRepository
{
    void Add(CustomsShipment shipment);

    void AddUpdateLog(CustomsUpdateLog updateLog);

    Task<(IReadOnlyList<CustomsShipment> Items, int TotalCount)> GetPagedAsync(
        ReportType reportType,
        CustomsShipmentStatus status,
        int pageNumber,
        int pageSize,
        string? search = null,
        Expression<Func<CustomsShipment, bool>>? filter = null,
        CancellationToken ct = default);

    Task<int> CountAsync(
        ReportType reportType,
        CustomsShipmentStatus status,
        Expression<Func<CustomsShipment, bool>> filter,
        CancellationToken ct = default);

    // Inactive (soft-deleted) archive, newest deactivated first. Always paged.
    Task<(IReadOnlyList<CustomsShipment> Items, int TotalCount)> GetInactivePagedAsync(
        CustomsInactiveFilter filter,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    // Distinct DeletedByName values for the Inactive archive "Inactive by" filter.
    Task<IReadOnlyList<string>> GetInactiveByNamesAsync(
        ReportType? reportType,
        CancellationToken ct = default);

    // One grouped query: counts per report, status and active/inactive.
    Task<IReadOnlyList<CustomsShipmentCountRow>> GetCountsAsync(
        CancellationToken ct = default);

    // Read by id, active or inactive (details drawer).
    Task<CustomsShipment?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    // Active or inactive (update history of the Inactive archive).
    Task<bool> ExistsIncludingInactiveAsync(
        Guid id,
        CancellationToken ct = default);

    Task<CustomsShipment?> GetForUpdateAsync(
        Guid id,
        CancellationToken ct = default);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken ct = default);

    Task<IReadOnlyList<CustomsUpdateLog>> GetUpdateLogsAsync(
        Guid shipmentId,
        CancellationToken ct = default);

    // Uniqueness checks (Open & Closed, same report)
    Task<bool> ShipmentNumberExistsAsync(
        ReportType reportType,
        string shipmentNumber,
        Guid? excludeId = null,
        CancellationToken ct = default);

    Task<bool> CertificateNumberExistsAsync(
        ReportType reportType,
        string certificateNumber,
        Guid? excludeId = null,
        CancellationToken ct = default);

    Task<bool> AcidNumberExistsAsync(
        string acidNumber,
        CancellationToken ct = default);

    Task<bool> MblExistsAsync(
        string mbl,
        CancellationToken ct = default);

    Task<bool> HblExistsAsync(
        string hbl,
        Guid? excludeId = null,
        CancellationToken ct = default);

    Task<bool> BookingNumberExistsAsync(
        string bookingNumber,
        CancellationToken ct = default);

    Task<bool> OceanExportCommercialInvoiceNumberExistsAsync(
        string commercialInvoiceNumber,
        CancellationToken ct = default);

    Task<bool> DomesticCommercialInvoiceNumberExistsAsync(
        string commercialInvoiceNumber,
        CancellationToken ct = default);
}
