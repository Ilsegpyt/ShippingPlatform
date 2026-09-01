using ClosedXML.Excel;
using Schedules.Application.Abstractions;
using Schedules.Domain.Schedule;

namespace Schedules.Infrastructure.Excel;

public sealed class ScheduleExcelWriter : IScheduleExcelWriter
{
    public byte[] Write(IReadOnlyList<Schedule> schedules)
    {
        using var workbook = new XLWorkbook();

        var worksheet = workbook.Worksheets.Add("Schedules");

        var headers = new[]
        {
            "RouteId",
            "Mode",
            "DepartureDate",
            "Vessel",
            "Origin",
            "DeparturePortCode",
            "DepartureCountry",
            "Destination",
            "ArrivalPortCode",
            "ArrivalCountry",
            "Carrier",
            "CarrierCode",
            "VoyageNumber",
            "Arrival",
            "TransitTime",
            "CutoffDate",
            "PortCutoffDate",
            "RateCurrency",
            "ContainerSize",
            "RateAmount",
            "RateRemarks",
            "ValidityDate",
            "FreeTimeAtPOD",
            "FreeTimeAtPOL",
            "TransshipmentData",
            "Notes",
            "CreatedAtUtc",
            "UpdatedAtUtc"
        };

        for (var i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
        }

        for (var rowIndex = 0; rowIndex < schedules.Count; rowIndex++)
        {
            var schedule = schedules[rowIndex];
            var excelRow = rowIndex + 2;

            worksheet.Cell(excelRow, 1).Value = schedule.RouteId;
            worksheet.Cell(excelRow, 2).Value = schedule.Mode.ToString();
            worksheet.Cell(excelRow, 3).Value = schedule.DepartureDate.ToDateTime(TimeOnly.MinValue);
            worksheet.Cell(excelRow, 4).Value = schedule.Vessel;
            worksheet.Cell(excelRow, 5).Value = schedule.Origin;
            worksheet.Cell(excelRow, 6).Value = schedule.DeparturePortCode;
            worksheet.Cell(excelRow, 7).Value = schedule.DepartureCountry;
            worksheet.Cell(excelRow, 8).Value = schedule.Destination;
            worksheet.Cell(excelRow, 9).Value = schedule.ArrivalPortCode;
            worksheet.Cell(excelRow, 10).Value = schedule.ArrivalCountry;
            worksheet.Cell(excelRow, 11).Value = schedule.Carrier;
            worksheet.Cell(excelRow, 12).Value = schedule.CarrierCode;
            worksheet.Cell(excelRow, 13).Value = schedule.VoyageNumber;
            worksheet.Cell(excelRow, 14).Value = schedule.Arrival.ToDateTime(TimeOnly.MinValue);
            worksheet.Cell(excelRow, 15).Value = schedule.TransitTime;
            worksheet.Cell(excelRow, 16).Value = schedule.CutoffDate.ToDateTime(TimeOnly.MinValue);
            worksheet.Cell(excelRow, 17).Value = schedule.PortCutoffDate.ToDateTime(TimeOnly.MinValue);
            worksheet.Cell(excelRow, 18).Value = schedule.RateCurrency;
            worksheet.Cell(excelRow, 19).Value = GetContainerSizeCode(schedule.ContainerSize);
            worksheet.Cell(excelRow, 20).Value = schedule.RateAmount;
            worksheet.Cell(excelRow, 21).Value = schedule.RateRemarks;
            worksheet.Cell(excelRow, 22).Value = schedule.ValidityDate.ToDateTime(TimeOnly.MinValue);
            worksheet.Cell(excelRow, 23).Value = schedule.FreeTimeAtPOD;
            worksheet.Cell(excelRow, 24).Value = schedule.FreeTimeAtPOL;
            worksheet.Cell(excelRow, 25).Value = schedule.TransshipmentData;
            worksheet.Cell(excelRow, 26).Value = schedule.Notes;
            worksheet.Cell(excelRow, 27).Value = schedule.CreatedAtUtc;
            worksheet.Cell(excelRow, 28).Value = schedule.UpdatedAtUtc;
        }

        worksheet.Row(1).Style.Font.Bold = true;

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        return stream.ToArray();
    }

    private static string GetContainerSizeCode(ContainerSize containerSize)
    {
        return containerSize switch
        {
            ContainerSize.Dry20Standard => "20GP",
            ContainerSize.Dry40Standard => "40GP",
            ContainerSize.Dry40High => "40HC",
            ContainerSize.Dry45High => "45HC",
            ContainerSize.Reefer20Standard => "20RF",
            ContainerSize.Reefer40High => "40RF",
            ContainerSize.OpenTop20 => "20OT",
            ContainerSize.OpenTop40 => "40OT",
            ContainerSize.OpenTop40High => "40OT HC",
            ContainerSize.Flat20 => "20FR",
            ContainerSize.Flat40Standard => "40FR",
            ContainerSize.Flat40High => "40FR HC",
            ContainerSize.Tank20 => "20TK",
            ContainerSize.Tank40 => "40TK",
            _ => containerSize.ToString()
        };
    }
}