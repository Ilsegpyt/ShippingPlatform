using BuildingBlocks.Application;
using MediatR;

namespace Schedules.Application.Schedules.ImportSchedules;

public sealed record ImportSchedulesCommand(
    Stream FileStream)
    : IRequest<Result<ImportSchedulesResult>>;

public sealed record ImportSchedulesResult(
    int TotalRows,
    int ImportedRows,
    IReadOnlyList<ImportScheduleImportError> Errors);

public sealed record ImportScheduleImportError(
    int RowNumber,
    IReadOnlyList<string> Errors);