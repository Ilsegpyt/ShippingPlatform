using BuildingBlocks.Application;
using MediatR;
using Transporters.Application.Abstractions.Repositories;
using Transporters.Domain.Entities;

namespace Transporters.Application.Ports.GetPortCoverageById;

public sealed class GetPortCoverageByIdQueryHandler(
    ITransporterRepository transporterRepository)
    : IRequestHandler<
        GetPortCoverageByIdQuery,
        Result<TransporterPortCoverage>>
{
    public async Task<Result<TransporterPortCoverage>> Handle(
        GetPortCoverageByIdQuery query,
        CancellationToken ct)
    {
        var portCoverage =
            await transporterRepository.GetPortCoverageByIdAsync(
                query.TransporterId,
                query.PortCoverageId,
                ct);

        if (portCoverage is null)
            return Result.Failure<TransporterPortCoverage>(
                "Port coverage was not found.");

        return Result.Success(portCoverage);
    }
}
