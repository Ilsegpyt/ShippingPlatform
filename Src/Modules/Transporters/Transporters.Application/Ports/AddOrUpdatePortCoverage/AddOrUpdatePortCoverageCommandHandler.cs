using BuildingBlocks.Application;
using MediatR;
using Transporters.Application.Abstractions;
using Transporters.Application.Abstractions.Repositories;
using Transporters.Domain.Entities;

namespace Transporters.Application.Ports.AddOrUpdatePortCoverage;

public sealed class AddOrUpdatePortCoverageCommandHandler(
    ITransporterRepository transporterRepository,
    ITransportersUnitOfWork transportersUnitOfWork)
    : IRequestHandler<AddOrUpdatePortCoverageCommand, Result>
{
    public async Task<Result> Handle(
        AddOrUpdatePortCoverageCommand cmd,
        CancellationToken ct)
    {
        if (!await transporterRepository.ExistsAsync(cmd.TransporterId, ct))
            return Result.Failure("Transporter was not found.");

        var portCoverage = await transporterRepository.GetPortCoverageForUpdateAsync(
            cmd.TransporterId,
            cmd.PortName,
            ct);

        if (portCoverage is null)
        {
            portCoverage = new TransporterPortCoverage
            {
                TransporterId = cmd.TransporterId,
                PortName = cmd.PortName
            };

            transporterRepository.AddPortCoverage(portCoverage);
        }

        portCoverage.FreeHours = cmd.FreeHours;
        portCoverage.DelayRatePerHour = cmd.DelayRatePerHour;
        portCoverage.OfficialReceiptNotes = cmd.OfficialReceiptNotes;
        portCoverage.GeneralNotes = cmd.GeneralNotes;
        portCoverage.Status = "Draft";
        portCoverage.UpdatedAtUtc = DateTime.UtcNow;

        portCoverage.LaneRates.Clear();

        foreach (var lane in cmd.LaneRates)
        {
            portCoverage.LaneRates.Add(new TransporterLaneRate
            {
                Destination = lane.Destination,
                EquipmentType = lane.EquipmentType,
                Price = lane.Price,
                Notes = lane.Notes
            });
        }

        await transportersUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
