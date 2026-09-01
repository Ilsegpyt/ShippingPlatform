using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Schedules.Application.Abstractions;
using Schedules.Domain.Schedule;

namespace Schedules.Infrastructure.Persistence.Repositories;

public sealed class ScheduleRepository(
    SchedulesDbContext context)
    : IScheduleRepository
{
    public async Task AddAsync(
        Schedule schedule,
        CancellationToken ct)
    {
        await context.Schedules.AddAsync(schedule, ct);
    }

    public async Task<Schedule?> GetByIdAsync(
        Guid id,
        CancellationToken ct)
    {
        return await context.Schedules
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public void Remove(Schedule schedule)
    {
        context.Schedules.Remove(schedule);
    }
    public async Task<IReadOnlyList<Schedule>> SearchAsync(
    string origin,
    string destination,
    DateOnly departureDate,
    ContainerSize containerSize,
    CancellationToken ct)
    {
        var schedules = context.Schedules
            .Where(x =>
                x.Origin == origin &&
                x.Destination == destination &&
                x.ContainerSize == containerSize &&
                x.DepartureDate >= departureDate);

        var nearestDepartureDate = await schedules
            .OrderBy(x => x.DepartureDate)
            .Select(x => (DateOnly?)x.DepartureDate)
            .FirstOrDefaultAsync(ct);

        if (nearestDepartureDate is null)
            return [];

        return await schedules
            .Where(x => x.DepartureDate == nearestDepartureDate.Value)
            .OrderBy(x => x.RateAmount)
            .ToListAsync(ct);
    }
    public async Task<IReadOnlyList<Schedule>> GetAllAsync(
    CancellationToken ct)
    {
        return await context.Schedules
            .AsNoTracking()
            .ToListAsync(ct);
    }
}
