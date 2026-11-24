using BuildingBlocks.Application;
using MediatR;

namespace Transporters.Application.Ports.ApprovePortRates;

public sealed record ApprovePortRatesCommand(Guid PortCoverageId)
    : IRequest<Result>;
