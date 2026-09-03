namespace Schedules.Contracts;

public interface IScheduleQueryService
{
    Task<ScheduleSearchResult?> GetByIdAsync(
        Guid scheduleId,
        CancellationToken ct);
}