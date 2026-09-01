using Schedules.Domain.Schedule;

namespace Schedules.Application.Schedules.ImportSchedules;

public sealed class ImportScheduleRowParser
{
    private static readonly Dictionary<string, ContainerSize> ContainerSizeMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["20GP"] = ContainerSize.Dry20Standard,
            ["40GP"] = ContainerSize.Dry40Standard,
            ["40HC"] = ContainerSize.Dry40High,
            ["45HC"] = ContainerSize.Dry45High,
            ["20RF"] = ContainerSize.Reefer20Standard,
            ["40RF"] = ContainerSize.Reefer40High,
            ["20OT"] = ContainerSize.OpenTop20,
            ["40OT"] = ContainerSize.OpenTop40,
            ["40OT HC"] = ContainerSize.OpenTop40High,
            ["20FR"] = ContainerSize.Flat20,
            ["40FR"] = ContainerSize.Flat40Standard,
            ["40FR HC"] = ContainerSize.Flat40High,
            ["20TK"] = ContainerSize.Tank20,
            ["40TK"] = ContainerSize.Tank40
        };

    public ImportScheduleRowResult Parse(ImportScheduleRawRow raw)
    {
        var errors = new List<string>();

        if (!Enum.TryParse<ScheduleMode>(
                raw.Mode,
                true,
                out var mode))
        {
            errors.Add("Mode is invalid.");
        }

        if (!DateOnly.TryParse(
                raw.DepartureDate,
                out var departureDate))
        {
            errors.Add("DepartureDate is invalid.");
        }

        if (!DateOnly.TryParse(
                raw.Arrival,
                out var arrival))
        {
            errors.Add("Arrival is invalid.");
        }

        if (!TimeSpan.TryParse(
                raw.TransitTime,
                out var transitTime))
        {
            errors.Add("TransitTime is invalid.");
        }

        if (!DateOnly.TryParse(
                raw.CutoffDate,
                out var cutoffDate))
        {
            errors.Add("CutoffDate is invalid.");
        }

        if (!decimal.TryParse(
                raw.RateAmount,
                out var rateAmount))
        {
            errors.Add("RateAmount is invalid.");
        }

        if (!DateOnly.TryParse(
                raw.ValidityDate,
                out var validityDate))
        {
            errors.Add("ValidityDate is invalid.");
        }

        if (!int.TryParse(
                raw.FreeTimeAtPOD,
                out var freeTimeAtPOD))
        {
            errors.Add("FreeTimeAtPOD is invalid.");
        }

        if (!int.TryParse(
                raw.FreeTimeAtPOL,
                out var freeTimeAtPOL))
        {
            errors.Add("FreeTimeAtPOL is invalid.");
        }

        ContainerSize containerSize = default;

        if (string.IsNullOrWhiteSpace(raw.ContainerSize) ||
            !ContainerSizeMap.TryGetValue(
                raw.ContainerSize.Trim(),
                out containerSize))
        {
            errors.Add("ContainerSize is invalid.");
        }

        if (errors.Count > 0)
        {
            return new ImportScheduleRowResult(
                raw.RowNumber,
                null,
                errors);
        }

        var row = new ImportScheduleRow(
            raw.RouteId,
            mode,
            departureDate,
            raw.Vessel!,
            raw.Origin!,
            raw.DeparturePortCode!,
            raw.DepartureCountry!,
            raw.Destination!,
            raw.ArrivalPortCode!,
            raw.ArrivalCountry!,
            raw.Carrier!,
            raw.CarrierCode!,
            raw.VoyageNumber!,
            arrival,
            transitTime,
            cutoffDate,
            raw.RateCurrency!,
            containerSize,
            rateAmount,
            raw.RateRemarks,
            validityDate,
            freeTimeAtPOD,
            freeTimeAtPOL,
            raw.TransshipmentData,
            raw.Notes);

        return new ImportScheduleRowResult(
            raw.RowNumber,
            row,
            errors);
    }
}