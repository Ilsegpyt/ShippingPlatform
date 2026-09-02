using BuildingBlocks.Application;
using MediatR;
using Schedules.Contracts;

namespace Schedules.Application.Schedules.MultiRouteSearch;

public sealed record MultiRouteSearchItem(
    string Origin,
    string Destination,
    DateOnly DepartureDate,
    string ContainerSize);

public sealed record MultiRouteSearchQuery(
    IReadOnlyList<MultiRouteSearchItem> Routes)
    : IRequest<Result<IReadOnlyList<ScheduleSearchResult>>>;