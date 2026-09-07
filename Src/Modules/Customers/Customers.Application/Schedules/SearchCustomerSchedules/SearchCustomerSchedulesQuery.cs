using BuildingBlocks.Application;
using MediatR;
using Schedules.Contracts;

namespace Customers.Application.Schedules.SearchCustomerSchedules;

public sealed record SearchCustomerSchedulesQuery(
    string Origin,
    string Destination,
    DateOnly DepartureDate,
    string ContainerSize,
    Guid CustomerId
) : IRequest<Result<IReadOnlyList<ScheduleSearchResult>>>;

