using BuildingBlocks.Application;
using MediatR;

namespace Transporters.Application.Ports.AddOrUpdatePortCoverage;

public sealed record AddOrUpdatePortCoverageCommand(
    Guid TransporterId,
    string PortName,
    int FreeHours,
    decimal DelayRatePerHour,
    string? OfficialReceiptNotes,
    string? GeneralNotes,
    IReadOnlyList<LaneRateItem> LaneRates)
    : IRequest<Result>;

public sealed record LaneRateItem(
    string Destination,
    string EquipmentType,
    decimal Price,
    string? Notes);
