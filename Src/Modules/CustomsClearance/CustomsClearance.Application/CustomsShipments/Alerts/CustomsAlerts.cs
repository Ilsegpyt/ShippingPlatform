using System.Linq.Expressions;
using CustomsClearance.Domain.Entities;
using CustomsClearance.Domain.Enums;

namespace CustomsClearance.Application.CustomsShipments.Alerts;

// Time-based alerts of Open Ocean Import shipments, computed on read (no stored state, no jobs).
//   Alert 1 (Eta):      48h before EtaDate, until AtaDate is entered or the shipment is closed.
//   Alert 4 (FreeTime): 48h before DateOfFreeTimeEnd, while the shipment is Open.
// An alert starts on (date - 2 days), Cairo time. A start on Friday or Saturday moves back to Thursday.
public enum CustomsAlertFilter
{
    Any,
    EtaSoon,
    EtaOverdue,
    FreeTimeSoon,
    FreeTimeExpired,
}

public static class CustomsAlertLevels
{
    public const string Soon = "soon";
    public const string Today = "today";
    public const string Overdue = "overdue";
    public const string Expired = "expired";
}

// Per-row alert state; a null level means no alert.
public sealed record CustomsShipmentAlerts(string? Eta, string? FreeTime);

public sealed record CustomsAlertsSummary(
    DateOnly Today,
    int EtaSoon,
    int EtaOverdue,
    int FreeTimeSoon,
    int FreeTimeExpired,
    int Total);

public static class CustomsAlerts
{
    public static DateOnly CairoToday(TimeProvider clock) => CustomsCairoTime.Today(clock);

    // First day an alert for `date` is shown.
    public static DateOnly StartDay(DateOnly date)
    {
        var start = date.AddDays(-2);
        return start.DayOfWeek switch
        {
            DayOfWeek.Friday => start.AddDays(-1),
            DayOfWeek.Saturday => start.AddDays(-2),
            _ => start,
        };
    }

    // Latest date whose alert has started by `today` (StartDay is monotonic, so alert active <=> date <= Cutoff).
    // Thursday covers up to Monday; Friday up to Monday; any other day covers today + 2.
    public static DateOnly Cutoff(DateOnly today) => today.DayOfWeek switch
    {
        DayOfWeek.Thursday => today.AddDays(4),
        DayOfWeek.Friday => today.AddDays(3),
        _ => today.AddDays(2),
    };

    public static CustomsShipmentAlerts? Evaluate(CustomsShipment shipment, DateOnly today)
    {
        var d = shipment.OceanImportDetails;
        if (shipment.ReportType != ReportType.OceanImport ||
            shipment.Status != CustomsShipmentStatus.Open ||
            d is null)
            return null;

        var cutoff = Cutoff(today);

        string? eta = null;
        if (d.AtaDate is null)
        {
            if (d.EtaDate < today) eta = CustomsAlertLevels.Overdue;
            else if (d.EtaDate == today) eta = CustomsAlertLevels.Today;
            else if (d.EtaDate <= cutoff) eta = CustomsAlertLevels.Soon;
        }

        string? freeTime = null;
        if (d.DateOfFreeTimeEnd is { } end)
        {
            if (end < today) freeTime = CustomsAlertLevels.Expired;
            else if (end == today) freeTime = CustomsAlertLevels.Today;
            else if (end <= cutoff) freeTime = CustomsAlertLevels.Soon;
        }

        return new CustomsShipmentAlerts(eta, freeTime);
    }

    // SQL-translatable filter over Open Ocean Import shipments.
    public static Expression<Func<CustomsShipment, bool>> Predicate(CustomsAlertFilter filter, DateOnly today)
    {
        var cutoff = Cutoff(today);

        return filter switch
        {
            CustomsAlertFilter.EtaSoon => x =>
                x.OceanImportDetails != null &&
                x.OceanImportDetails.AtaDate == null &&
                x.OceanImportDetails.EtaDate >= today &&
                x.OceanImportDetails.EtaDate <= cutoff,

            CustomsAlertFilter.EtaOverdue => x =>
                x.OceanImportDetails != null &&
                x.OceanImportDetails.AtaDate == null &&
                x.OceanImportDetails.EtaDate < today,

            CustomsAlertFilter.FreeTimeSoon => x =>
                x.OceanImportDetails != null &&
                x.OceanImportDetails.DateOfFreeTimeEnd >= today &&
                x.OceanImportDetails.DateOfFreeTimeEnd <= cutoff,

            CustomsAlertFilter.FreeTimeExpired => x =>
                x.OceanImportDetails != null &&
                x.OceanImportDetails.DateOfFreeTimeEnd < today,

            _ => x =>
                x.OceanImportDetails != null && (
                    (x.OceanImportDetails.AtaDate == null && x.OceanImportDetails.EtaDate <= cutoff) ||
                    x.OceanImportDetails.DateOfFreeTimeEnd <= cutoff),
        };
    }
}
