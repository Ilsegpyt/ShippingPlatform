using CustomsClearance.Domain.Enums;

namespace CustomsClearance.Application.CustomsShipments.Finance;

// Customs Finance screen: Open Ocean Import shipments only, all day comparisons in Cairo time.
//   Forecast : Today <= EtaDate <= Today + 15
//   Request  : (no decision and EtaDate = Today + 1) or Pending (restored) or Deferred with DeferredUntilDate <= Today
//   Approved : Approved
//   Excluded : OnHold, or Deferred with DeferredUntilDate > Today
public enum CustomsFinanceTab
{
    Forecast = 1,
    Request = 2,
    Approved = 3,
    Excluded = 4
}

public enum CustomsFinanceExcludeMode
{
    Defer = 1,
    OnHold = 2
}

public static class CustomsFinanceWindow
{
    public const int ForecastDays = 15;
}

// Egp / Eur / Usd are always null for now: extension point for the future accounting integration.
public sealed record CustomsFinanceRow(
    Guid Id,
    string? ShipmentNumber,
    Guid ClientId,
    string? CertificateNumber,
    string Mbl,
    DateOnly EtaDate,
    CustomsFinanceStatus? FinanceStatus,
    DateOnly? DeferredUntilDate,
    string? Note,
    DateTime? ApprovedAtUtc,
    string? ApprovedByName,
    DateTime? UpdatedAtUtc,
    string? UpdatedByName,
    decimal? Egp,
    decimal? Eur,
    decimal? Usd);

// Sums of the tab's amounts (null while no amounts exist).
public sealed record CustomsFinanceTotals(decimal? Egp, decimal? Eur, decimal? Usd);

public sealed record CustomsFinanceList(
    IReadOnlyList<CustomsFinanceRow> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    DateOnly Today,
    CustomsFinanceTotals Totals);

public sealed record CustomsFinanceCounts(DateOnly Today, int Forecast, int Request, int Approved, int Excluded);
