namespace Schedules.Contracts;

// Get a Specific Schedule Based on ID
public interface IScheduleQueryService
{
    Task<ScheduleSearchResult?> GetByIdAsync(Guid scheduleId, CancellationToken ct);
}