using BuildingBlocks.Application;
using MediatR;
using Schedules.Application.Abstractions;
using Schedules.Domain.Schedule;

namespace Schedules.Application.Schedules.ImportSchedules;

public sealed class ImportSchedulesCommandHandler(
    IScheduleExcelReader excelReader,
    ImportScheduleRowParser parser,
    ImportScheduleRowValidator validator,
    IScheduleRepository scheduleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<
        ImportSchedulesCommand,
        Result<ImportSchedulesResult>>
{
    public async Task<Result<ImportSchedulesResult>> Handle(
        ImportSchedulesCommand command,
        CancellationToken ct)
    {
        var rawRows = excelReader.Read(command.FileStream);

        var errors = new List<ImportScheduleImportError>();
        var validRows = new List<ImportScheduleRow>();

        foreach (var rawRow in rawRows)
        {
            var parseResult = parser.Parse(rawRow);

            if (parseResult.Row is null)
            {
                errors.Add(new ImportScheduleImportError(
                    parseResult.RowNumber,
                    parseResult.Errors));

                continue;
            }

            var validationResult = await validator.ValidateAsync(
                parseResult.Row,
                ct);

            if (!validationResult.IsValid)
            {
                errors.Add(new ImportScheduleImportError(
                    rawRow.RowNumber,
                    validationResult.Errors
                        .Select(x => x.ErrorMessage)
                        .ToList()));

                continue;
            }

            validRows.Add(parseResult.Row);
        }

        // أي Error = مفيش أي Insert
        if (errors.Count > 0)
        {
            return Result.Success(
                new ImportSchedulesResult(
                    rawRows.Count,
                    0,
                    errors));
        }

        foreach (var row in validRows)
        {
            var schedule = Schedule.Create(
                             row.RouteId,
                             row.Mode,
                             row.DepartureDate,
                             row.Vessel,
                             row.Origin,
                             row.DeparturePortCode,
                             row.DepartureCountry,
                             row.Destination,
                             row.ArrivalPortCode,
                             row.ArrivalCountry,
                             row.Carrier,
                             row.CarrierCode,
                             row.VoyageNumber,
                             row.Arrival,
                             row.TransitTime,
                             row.CutoffDate,
                             row.RateCurrency,
                             row.ContainerSize,
                             row.RateAmount,
                             row.RateRemarks,
                             row.ValidityDate,
                             row.FreeTimeAtPOD,
                             row.FreeTimeAtPOL,
                             row.TransshipmentData,
                             row.Notes);

            await scheduleRepository.AddAsync(schedule, ct);
        }

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(
            new ImportSchedulesResult(
                rawRows.Count,
                validRows.Count,
                []));
    }
}