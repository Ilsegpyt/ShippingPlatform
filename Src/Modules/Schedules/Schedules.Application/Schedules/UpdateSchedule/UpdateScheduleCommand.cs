using BuildingBlocks.Application;
using MediatR;
using Schedules.Domain.Schedule;

namespace Schedules.Application.Schedules.UpdateSchedule;

public sealed record UpdateScheduleCommand(
    Guid Id,
    string? RouteId = null,
    ScheduleMode? Mode = null,
    DateOnly? DepartureDate = null,
    string? Vessel = null,
    string? Origin = null,
    string? DeparturePortCode = null,
    string? DepartureCountry = null,
    string? Destination = null,
    string? ArrivalPortCode = null,
    string? ArrivalCountry = null,
    string? Carrier = null,
    string? CarrierCode = null,
    string? VoyageNumber = null,
    DateOnly? Arrival = null,
    TimeSpan? TransitTime = null,
    DateOnly? CutoffDate = null,
    string? RateCurrency = null,
    ContainerSize? ContainerSize = null,
    decimal? RateAmount = null,
    string? RateRemarks = null,
    DateOnly? ValidityDate = null,
    int? FreeTimeAtPOD = null,
    int? FreeTimeAtPOL = null,
    string? TransshipmentData = null,
    string? Notes = null
) : IRequest<Result>;