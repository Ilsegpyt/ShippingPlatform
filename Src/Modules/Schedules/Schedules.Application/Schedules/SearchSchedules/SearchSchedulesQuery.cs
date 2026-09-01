using BuildingBlocks.Application;
using MediatR;
using Schedules.Domain.Schedule;

namespace Schedules.Application.Schedules.SearchSchedules;

public sealed record SearchSchedulesQuery(
    string Origin,
    string Destination,
    DateOnly DepartureDate,
    ContainerSize ContainerSize
) : IRequest<Result<IReadOnlyList<SearchScheduleResponse>>>;

public sealed record SearchScheduleResponse(
    Guid Id,
    string? RouteId,
    ScheduleMode Mode,
    DateOnly DepartureDate,
    string Vessel,
    string Origin,
    string DeparturePortCode,
    string Destination,
    string ArrivalPortCode,
    string Carrier,
    string CarrierCode,
    string VoyageNumber,
    DateOnly Arrival,
    TimeSpan TransitTime,
    decimal RateAmount,
    string RateCurrency,
    ContainerSize ContainerSize
);
