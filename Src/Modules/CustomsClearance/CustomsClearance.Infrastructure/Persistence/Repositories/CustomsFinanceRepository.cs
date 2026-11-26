using CustomsClearance.Application.Abstractions.Repositories;
using CustomsClearance.Application.CustomsShipments.Finance;
using CustomsClearance.Domain.Entities;
using CustomsClearance.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CustomsClearance.Infrastructure.Persistence.Repositories;

public sealed class CustomsFinanceRepository(CustomsClearanceDbContext dbContext)
    : ICustomsFinanceRepository
{
    private sealed class Source
    {
        public CustomsShipment Shipment { get; init; } = null!;
        public DateOnly EtaDate { get; init; }
        public string Mbl { get; init; } = null!;
        public CustomsFinanceRequest? Finance { get; init; }
    }

    public void Add(CustomsFinanceRequest request)
    {
        dbContext.CustomsFinanceRequests.Add(request);
    }

    public Task<CustomsFinanceRequest?> GetForUpdateAsync(Guid shipmentId, CancellationToken ct = default) =>
        dbContext.CustomsFinanceRequests.FirstOrDefaultAsync(x => x.ShipmentId == shipmentId, ct);

    public Task<bool> IsOpenOceanImportAsync(Guid shipmentId, CancellationToken ct = default) =>
        dbContext.CustomsShipments.AnyAsync(
            x => x.Id == shipmentId
                 && x.ReportType == ReportType.OceanImport
                 && x.Status == CustomsShipmentStatus.Open,
            ct);

    public async Task<(IReadOnlyList<CustomsFinanceRow> Items, int TotalCount)> GetPagedAsync(
        CustomsFinanceTab tab,
        DateOnly today,
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = ForTab(tab, today);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x =>
                (x.Shipment.ShipmentNumber != null && x.Shipment.ShipmentNumber.Contains(term)) ||
                (x.Shipment.CertificateNumber != null && x.Shipment.CertificateNumber.Contains(term)) ||
                x.Mbl.Contains(term));
        }

        var totalCount = await query.CountAsync(ct);

        var ordered = tab switch
        {
            CustomsFinanceTab.Approved => query
                .OrderByDescending(x => x.Finance!.ApprovedAtUtc)
                .ThenBy(x => x.EtaDate),
            CustomsFinanceTab.Excluded => query
                .OrderByDescending(x => x.Finance!.UpdatedAtUtc)
                .ThenBy(x => x.EtaDate),
            _ => query
                .OrderBy(x => x.EtaDate)
                .ThenBy(x => x.Shipment.ShipmentNumber),
        };

        var items = await ordered
            .ThenBy(x => x.Shipment.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new CustomsFinanceRow(
                x.Shipment.Id,
                x.Shipment.ShipmentNumber,
                x.Shipment.ClientId,
                x.Shipment.CertificateNumber,
                x.Mbl,
                x.EtaDate,
                x.Finance == null ? null : x.Finance.Status,
                x.Finance == null ? null : x.Finance.DeferredUntilDate,
                x.Finance == null ? null : x.Finance.Note,
                x.Finance == null ? null : x.Finance.ApprovedAtUtc,
                x.Finance == null ? null : x.Finance.ApprovedByName,
                x.Finance == null ? null : x.Finance.UpdatedAtUtc,
                x.Finance == null ? null : x.Finance.UpdatedByName,
                null,
                null,
                null))
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public Task<int> CountAsync(CustomsFinanceTab tab, DateOnly today, CancellationToken ct = default) =>
        ForTab(tab, today).CountAsync(ct);

    // Open Ocean Import shipments (inactive ones are hidden by the global query filter) with their finance row, if any.
    private IQueryable<Source> ForTab(CustomsFinanceTab tab, DateOnly today)
    {
        var query =
            from s in dbContext.CustomsShipments.AsNoTracking()
            where s.ReportType == ReportType.OceanImport && s.Status == CustomsShipmentStatus.Open
            join f in dbContext.CustomsFinanceRequests.AsNoTracking() on s.Id equals f.ShipmentId into finance
            from f in finance.DefaultIfEmpty()
            select new Source
            {
                Shipment = s,
                EtaDate = s.OceanImportDetails!.EtaDate,
                Mbl = s.OceanImportDetails.Mbl,
                Finance = f,
            };

        var tomorrow = today.AddDays(1);
        var forecastEnd = today.AddDays(CustomsFinanceWindow.ForecastDays);

        return tab switch
        {
            CustomsFinanceTab.Forecast => query.Where(x => x.EtaDate >= today && x.EtaDate <= forecastEnd),
            CustomsFinanceTab.Request => query.Where(x =>
                (x.Finance == null && x.EtaDate == tomorrow) ||
                (x.Finance != null && x.Finance.Status == CustomsFinanceStatus.Pending) ||
                (x.Finance != null && x.Finance.Status == CustomsFinanceStatus.Deferred && x.Finance.DeferredUntilDate <= today)),
            CustomsFinanceTab.Approved => query.Where(x =>
                x.Finance != null && x.Finance.Status == CustomsFinanceStatus.Approved),
            CustomsFinanceTab.Excluded => query.Where(x =>
                x.Finance != null &&
                (x.Finance.Status == CustomsFinanceStatus.OnHold ||
                 (x.Finance.Status == CustomsFinanceStatus.Deferred && x.Finance.DeferredUntilDate > today))),
            _ => query.Where(_ => false),
        };
    }
}
