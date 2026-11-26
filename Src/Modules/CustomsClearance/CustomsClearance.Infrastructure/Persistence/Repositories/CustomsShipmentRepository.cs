using System.Linq.Expressions;
using CustomsClearance.Application.Abstractions.Repositories;
using CustomsClearance.Application.CustomsShipments.Inactive;
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
        Expression<Func<CustomsShipment, bool>>? filter = null,
        CancellationToken ct = default)
    {
        var query = dbContext.CustomsShipments
            .AsNoTracking()
            .Where(x => x.ReportType == reportType && x.Status == status);

        if (filter is not null)
            query = query.Where(filter);

        query = ApplySearch(query, search);

        // Open Ocean Import: nearest ETA first, so shipments needing action stay on top.
        query = reportType == ReportType.OceanImport && status == CustomsShipmentStatus.Open
            ? query
                .OrderBy(x => x.OceanImportDetails!.EtaDate)
                .ThenByDescending(x => x.CreatedAtUtc)
            : query.OrderByDescending(x => x.CreatedAtUtc);

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

    public async Task<int> CountAsync(
        ReportType reportType,
        CustomsShipmentStatus status,
        Expression<Func<CustomsShipment, bool>> filter,
        CancellationToken ct = default)
    {
        return await dbContext.CustomsShipments
            .Where(x => x.ReportType == reportType && x.Status == status)
            .Where(filter)
            .CountAsync(ct);
    }

    private static IQueryable<CustomsShipment> ApplySearch(
        IQueryable<CustomsShipment> query,
        string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return query;

        var s = search.Trim();
        return query.Where(x =>
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

    public async Task<(IReadOnlyList<CustomsShipment> Items, int TotalCount)> GetInactivePagedAsync(
        CustomsInactiveFilter filter,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = dbContext.CustomsShipments
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.IsDeleted);

        if (filter.ReportType is { } reportType)
            query = query.Where(x => x.ReportType == reportType);

        if (!string.IsNullOrWhiteSpace(filter.InactiveBy))
        {
            var by = filter.InactiveBy.Trim();
            query = query.Where(x => x.DeletedByName == by);
        }

        if (filter.DateType == CustomsInactiveDateType.Created)
        {
            if (filter.FromUtc is { } from)
                query = query.Where(x => x.CreatedAtUtc >= from);
            if (filter.ToUtcExclusive is { } to)
                query = query.Where(x => x.CreatedAtUtc < to);
        }
        else
        {
            if (filter.FromUtc is { } from)
                query = query.Where(x => (x.DeletedAtUtc ?? x.CreatedAtUtc) >= from);
            if (filter.ToUtcExclusive is { } to)
                query = query.Where(x => (x.DeletedAtUtc ?? x.CreatedAtUtc) < to);
        }

        query = ApplySearch(query, filter.Search)
            .OrderByDescending(x => x.DeletedAtUtc ?? x.CreatedAtUtc)
            .ThenByDescending(x => x.Id);

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

    public async Task<IReadOnlyList<string>> GetInactiveByNamesAsync(
        ReportType? reportType,
        CancellationToken ct = default)
    {
        var query = dbContext.CustomsShipments
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.IsDeleted && x.DeletedByName != null && x.DeletedByName != "");

        if (reportType is { } rt)
            query = query.Where(x => x.ReportType == rt);

        return await query
            .Select(x => x.DeletedByName!)
            .Distinct()
            .OrderBy(x => x)
            .Take(200)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CustomsShipmentCountRow>> GetCountsAsync(
        CancellationToken ct = default)
    {
        return await dbContext.CustomsShipments
            .IgnoreQueryFilters()
            .AsNoTracking()
            .GroupBy(x => new { x.ReportType, x.Status, x.IsDeleted })
            .Select(g => new CustomsShipmentCountRow(g.Key.ReportType, g.Key.Status, g.Key.IsDeleted, g.Count()))
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsIncludingInactiveAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await dbContext.CustomsShipments
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Id == id, ct);
    }

    public async Task<CustomsShipment?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await dbContext.CustomsShipments
            .IgnoreQueryFilters()
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
        return await dbContext.CustomsShipments
            .AnyAsync(x => x.OceanImportDetails != null && x.OceanImportDetails.AcidNumber == acidNumber, ct);
    }

    public async Task<bool> MblExistsAsync(
        string mbl,
        CancellationToken ct = default)
    {
        return await dbContext.CustomsShipments
            .AnyAsync(x => x.OceanImportDetails != null && x.OceanImportDetails.Mbl == mbl, ct);
    }

    public async Task<bool> HblExistsAsync(
        string hbl,
        Guid? excludeId = null,
        CancellationToken ct = default)
    {
        return await dbContext.CustomsShipments
            .AnyAsync(x => x.OceanImportDetails != null && x.OceanImportDetails.Hbl == hbl && x.Id != excludeId, ct);
    }

    public async Task<bool> BookingNumberExistsAsync(
        string bookingNumber,
        CancellationToken ct = default)
    {
        return await dbContext.CustomsShipments
            .AnyAsync(x => x.OceanExportDetails != null && x.OceanExportDetails.BookingNumber == bookingNumber, ct);
    }

    public async Task<bool> OceanExportCommercialInvoiceNumberExistsAsync(
        string commercialInvoiceNumber,
        CancellationToken ct = default)
    {
        return await dbContext.CustomsShipments
            .AnyAsync(x => x.OceanExportDetails != null && x.OceanExportDetails.CommercialInvoiceNumber == commercialInvoiceNumber, ct);
    }

    public async Task<bool> DomesticCommercialInvoiceNumberExistsAsync(
        string commercialInvoiceNumber,
        CancellationToken ct = default)
    {
        return await dbContext.CustomsShipments
            .AnyAsync(x => x.DomesticImportDetails != null && x.DomesticImportDetails.CommercialInvoiceNumber == commercialInvoiceNumber, ct);
    }
}
