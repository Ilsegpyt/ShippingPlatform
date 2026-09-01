using Schedules.Application.Schedules.ImportSchedules;

namespace Schedules.Application.Abstractions;

public interface IScheduleExcelReader
{
    List<ImportScheduleRawRow> Read(Stream stream);
}