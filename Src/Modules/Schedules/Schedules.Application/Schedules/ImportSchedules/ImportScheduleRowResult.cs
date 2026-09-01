namespace Schedules.Application.Schedules.ImportSchedules;

public sealed record ImportScheduleRowResult(
    int RowNumber,
    ImportScheduleRow? Row,
    IReadOnlyList<string> Errors);