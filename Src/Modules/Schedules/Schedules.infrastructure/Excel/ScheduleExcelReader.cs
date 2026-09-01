using ClosedXML.Excel;
using Schedules.Application.Abstractions;
using Schedules.Application.Schedules.ImportSchedules;

namespace Schedules.Infrastructure.Excel;

public sealed class ScheduleExcelReader : IScheduleExcelReader
{
    public List<ImportScheduleRawRow> Read(Stream stream)
    {
        using var workbook = new XLWorkbook(stream);

        var worksheet = workbook.Worksheets.First();

        var rows = new List<ImportScheduleRawRow>();

        foreach (var row in worksheet.RowsUsed().Skip(1))
        {
            rows.Add(new ImportScheduleRawRow(
                RowNumber: row.RowNumber(),
                RouteId: GetString(row, 1),
                Mode: GetString(row, 2),
                DepartureDate: GetDateString(row, 3),
                Vessel: GetString(row, 4),
                Origin: GetString(row, 5),
                DeparturePortCode: GetString(row, 6),
                DepartureCountry: GetString(row, 7),
                Destination: GetString(row, 8),
                ArrivalPortCode: GetString(row, 9),
                ArrivalCountry: GetString(row, 10),
                Carrier: GetString(row, 11),
                CarrierCode: GetString(row, 12),
                VoyageNumber: GetString(row, 13),
                Arrival: GetDateString(row, 14),
                TransitTime: GetTimeSpanString(row, 15),
                CutoffDate: GetDateString(row, 16),
                RateCurrency: GetString(row, 17),
                ContainerSize: GetString(row, 18),
                RateAmount: GetString(row, 19),
                RateRemarks: GetString(row, 20),
                ValidityDate: GetDateString(row, 21),
                FreeTimeAtPOD: GetString(row, 22),
                FreeTimeAtPOL: GetString(row, 23),
                TransshipmentData: GetString(row, 24),
                Notes: GetString(row, 25)
            ));
        }

        return rows;
    }

    private static string? GetString(IXLRow row, int column)
    {
        var value = row.Cell(column).GetString().Trim();

        return string.IsNullOrWhiteSpace(value)
            ? null
            : value;
    }

    private static string? GetDateString(IXLRow row, int column)
    {
        var cell = row.Cell(column);

        if (cell.IsEmpty())
            return null;

        if (cell.TryGetValue<DateTime>(out var dateTime))
            return DateOnly.FromDateTime(dateTime).ToString("yyyy-MM-dd");

        var value = cell.GetString().Trim();

        return string.IsNullOrWhiteSpace(value)
            ? null
            : value;
    }

    private static string? GetTimeSpanString(IXLRow row, int column)
    {
        var cell = row.Cell(column);

        if (cell.IsEmpty())
            return null;

        if (cell.TryGetValue<TimeSpan>(out var timeSpan))
            return timeSpan.ToString();

        var value = cell.GetString().Trim();

        return string.IsNullOrWhiteSpace(value)
            ? null
            : value;
    }
}