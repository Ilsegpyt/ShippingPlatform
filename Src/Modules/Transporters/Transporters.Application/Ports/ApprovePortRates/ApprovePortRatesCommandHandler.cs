using BuildingBlocks.Application;
using MediatR;
using Transporters.Application.Abstractions;
using Transporters.Application.Abstractions.Repositories;

namespace Transporters.Application.Ports.ApprovePortRates;

public sealed class ApprovePortRatesCommandHandler(
    ITransporterRepository transporterRepository,
    ITransportersUnitOfWork transportersUnitOfWork)
    : IRequestHandler<ApprovePortRatesCommand, Result>
{
    public async Task<Result> Handle(
        ApprovePortRatesCommand cmd,
        CancellationToken ct)
    {
        var portCoverage = await transporterRepository.GetPortCoverageForUpdateAsync(
            cmd.PortCoverageId,
            ct);

        if (portCoverage is null)
            return Result.Failure("Port coverage was not found.");

        if (portCoverage.Status == "Approved")
            return Result.Failure("Port rates are already approved.");

        portCoverage.Status = "Approved";
        portCoverage.UpdatedAtUtc = DateTime.UtcNow;

        await transportersUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
