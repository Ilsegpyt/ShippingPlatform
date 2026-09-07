using BuildingBlocks.Application;
using MediatR;
using Schedules.Contracts;

namespace Customers.Application.Schedules.SearchCustomerMultiSchedules;

public sealed record SearchCustomerMultiSchedulesQuery(IReadOnlyList<SearchCustomerMultiRouteItem> Routes, Guid CustomerId)
    : IRequest<Result<IReadOnlyList<ScheduleSearchResult>>>;

public sealed record SearchCustomerMultiRouteItem(
    string Origin,
    string Destination,
    DateOnly DepartureDate,
    string ContainerSize);

