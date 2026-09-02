namespace Schedules.Contracts;

public interface IScheduleSearchService
{
    Task<IReadOnlyList<ScheduleSearchResult>> SearchAsync(
        string origin,
        string destination,
        DateOnly departureDate,
        string containerSize,
        CancellationToken ct);
}

public sealed record ScheduleSearchResult(
    Guid Id,
    string? RouteId,
    string Mode,
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
    string ContainerSize);