using Schedules.Domain.Schedule;

namespace Schedules.Application.Abstractions;

public interface IScheduleExcelWriter
{
    byte[] Write(IReadOnlyList<Schedule> schedules);
}