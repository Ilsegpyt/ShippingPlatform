using CustomsClearance.Application.Abstractions.Repositories;
using CustomsClearance.Domain.Entities;
using CustomsClearance.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CustomsClearance.Infrastructure.Persistence.Repositories;

public sealed class CustomsShipmentRepository(CustomsClearanceDbContext dbContext)
    : ICustomsShipmentRepository
{
    public void Add(CustomsShipment shipment)
    {
        dbContext.CustomsShipments.Add(shipment);
    }

    public void AddUpdateLog(CustomsUpdateLog updateLog)
    {
        dbContext.CustomsUpdateLogs.Add(updateLog);
    }

    public async Task<(IReadOnlyList<CustomsShipment> Items, int TotalCount)> GetPagedAsync(
        ReportType reportType,
        CustomsShipmentStatus status,
        int pageNumber,
        int pageSize,
        string? search = null,
        CancellationToken ct = default)
    {
        var query = dbContext.CustomsShipments
            .AsNoTracking()
            .Where(x => x.ReportType == reportType && x.Status == status);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(x =>
                EF.Functions.Like(x.ShipmentNumber!, $"%{s}%") ||
                EF.Functions.Like(x.CertificateNumber!, $"%{s}%") ||
                EF.Functions.Like(x.LatestUpdate!, $"%{s}%") ||
                (x.OceanImportDetails != null && (
                    EF.Functions.Like(x.OceanImportDetails.AcidNumber!, $"%{s}%") ||
                    EF.Functions.Like(x.OceanImportDetails.Mbl, $"%{s}%") ||
                    EF.Functions.Like(x.OceanImportDetails.Hbl!, $"%{s}%"))) ||
                (x.OceanExportDetails != null && (
                    EF.Functions.Like(x.OceanExportDetails.BookingNumber, $"%{s}%") ||
                    EF.Functions.Like(x.OceanExportDetails.CommercialInvoiceNumber, $"%{s}%"))) ||
                (x.DomesticImportDetails != null &&
                    EF.Functions.Like(x.DomesticImportDetails.CommercialInvoiceNumber, $"%{s}%")));
        }

        query = query.OrderByDescending(x => x.CreatedAtUtc);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Include(x => x.OceanImportDetails)
            .Include(x => x.OceanExportDetails)
            .Include(x => x.DomesticImportDetails)
            .Include(x => x.Containers.OrderBy(c => c.Ordinal))
            .AsSplitQuery()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<CustomsShipment?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await dbContext.CustomsShipments
            .AsNoTracking()
            .Include(x => x.OceanImportDetails)
            .Include(x => x.OceanExportDetails)
            .Include(x => x.DomesticImportDetails)
            .Include(x => x.Containers.OrderBy(c => c.Ordinal))
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<CustomsShipment?> GetForUpdateAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await dbContext.CustomsShipments
            .Include(x => x.OceanImportDetails)
            .Include(x => x.OceanExportDetails)
            .Include(x => x.DomesticImportDetails)
            .Include(x => x.Containers)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await dbContext.CustomsShipments
            .AnyAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<CustomsUpdateLog>> GetUpdateLogsAsync(
        Guid shipmentId,
        CancellationToken ct = default)
    {
        return await dbContext.CustomsUpdateLogs
            .AsNoTracking()
            .Where(x => x.ShipmentId == shipmentId)
            .OrderByDescending(x => x.UpdatedAtUtc)
            .ToListAsync(ct);
    }

    public async Task<bool> ShipmentNumberExistsAsync(
        ReportType reportType,
        string shipmentNumber,
        Guid? excludeId = null,
        CancellationToken ct = default)
    {
        return await dbContext.CustomsShipments
            .AnyAsync(x =>
                x.ReportType == reportType &&
                x.ShipmentNumber == shipmentNumber &&
                x.Id != excludeId,
                ct);
    }

    public async Task<bool> CertificateNumberExistsAsync(
        ReportType reportType,
        string certificateNumber,
        Guid? excludeId = null,
        CancellationToken ct = default)
    {
        return await dbContext.CustomsShipments
            .AnyAsync(x =>
                x.ReportType == reportType &&
                x.CertificateNumber == certificateNumber &&
                x.Id != excludeId,
                ct);
    }

    public async Task<bool> AcidNumberExistsAsync(
        string acidNumber,
        CancellationToken ct = default)
    {
        return await dbContext.OceanImportDetails
            .AnyAsync(x => x.AcidNumber == acidNumber, ct);
    }

    public async Task<bool> MblExistsAsync(
        string mbl,
        CancellationToken ct = default)
    {
        return await dbContext.OceanImportDetails
            .AnyAsync(x => x.Mbl == mbl, ct);
    }

    public async Task<bool> HblExistsAsync(
        string hbl,
        Guid? excludeId = null,
        CancellationToken ct = default)
    {
        return await dbContext.OceanImportDetails
            .AnyAsync(x => x.Hbl == hbl && x.ShipmentId != excludeId, ct);
    }

    public async Task<bool> BookingNumberExistsAsync(
        string bookingNumber,
        CancellationToken ct = default)
    {
        return await dbContext.OceanExportDetails
            .AnyAsync(x => x.BookingNumber == bookingNumber, ct);
    }

    public async Task<bool> OceanExportCommercialInvoiceNumberExistsAsync(
        string commercialInvoiceNumber,
        CancellationToken ct = default)
    {
        return await dbContext.OceanExportDetails
            .AnyAsync(x => x.CommercialInvoiceNumber == commercialInvoiceNumber, ct);
    }

    public async Task<bool> DomesticCommercialInvoiceNumberExistsAsync(
        string commercialInvoiceNumber,
        CancellationToken ct = default)
    {
        return await dbContext.DomesticImportDetails
            .AnyAsync(x => x.CommercialInvoiceNumber == commercialInvoiceNumber, ct);
    }
}
