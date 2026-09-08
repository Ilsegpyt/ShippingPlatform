
namespace Tracking.Application.Tracking;

public sealed record ShipmentTrackingResponse(
    Guid ShipmentId,
    string ShipmentRef,
    string Status,
    string Origin,
    string Destination,
    string DeparturePortCode,
    string ArrivalPortCode,
    DateOnly DepartureDate,
    DateOnly ArrivalDate,
    TimeSpan TransitTime);