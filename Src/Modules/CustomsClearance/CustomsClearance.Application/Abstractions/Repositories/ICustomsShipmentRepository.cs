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
        CancellationToken ct = default);

    Task<CustomsShipment?> GetByIdAsync(
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
