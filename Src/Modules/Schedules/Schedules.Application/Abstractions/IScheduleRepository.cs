using Schedules.Domain.Schedule;

namespace Schedules.Application.Abstractions;

public interface IScheduleRepository
{
    Task AddAsync(
        Schedule schedule,
        CancellationToken ct);

    Task<Schedule?> GetByIdAsync(
        Guid id,
        CancellationToken ct);

    Task<IReadOnlyList<Schedule>> SearchAsync(
        string origin,
        string destination,
        DateOnly departureDate,
        ContainerSize containerSize,
        CancellationToken ct);

    void Remove(Schedule schedule);

    Task<IReadOnlyList<Schedule>> GetAllAsync(
    CancellationToken ct);
}