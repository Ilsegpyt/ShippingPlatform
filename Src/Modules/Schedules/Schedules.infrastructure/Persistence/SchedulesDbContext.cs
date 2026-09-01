
using Microsoft.EntityFrameworkCore;
using Schedules.Application.Abstractions;
using Schedules.Domain.Schedule;

namespace Schedules.Infrastructure.Persistence;

public sealed class SchedulesDbContext
    : DbContext, ISchedulesUnitOfWork
{
    public SchedulesDbContext(
        DbContextOptions<SchedulesDbContext> options)
        : base(options)
    {
    }

    public DbSet<Schedule> Schedules => Set<Schedule>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(
            typeof(SchedulesDbContext).Assembly);

        base.OnModelCreating(builder);
    }
}

