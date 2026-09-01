
using BuildingBlocks.Application;
using MediatR;
using Schedules.Domain.Schedule;

namespace Schedules.Application.Schedules.CreateSchedule;

public sealed record CreateScheduleCommand(
    string? RouteId,
    ScheduleMode Mode,
    DateOnly DepartureDate,
    string Vessel,
    string Origin,
    string DeparturePortCode,
    string DepartureCountry,
    string Destination,
    string ArrivalPortCode,
    string ArrivalCountry,
    string Carrier,
    string CarrierCode,
    string VoyageNumber,
    DateOnly Arrival,
    TimeSpan TransitTime,
    DateOnly CutoffDate,
    string RateCurrency,
    ContainerSize ContainerSize,
    decimal RateAmount,
    string? RateRemarks,
    DateOnly ValidityDate,
    int FreeTimeAtPOD,
    int FreeTimeAtPOL,
    string? TransshipmentData,
    string? Notes
) : IRequest<Result<Guid>>;

