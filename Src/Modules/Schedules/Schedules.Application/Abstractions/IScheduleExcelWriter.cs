using Schedules.Domain.Entities;

namespace Schedules.Application.Abstractions;

public interface IScheduleExcelWriter
{
    byte[] Write(IReadOnlyList<Schedule> schedules);
}