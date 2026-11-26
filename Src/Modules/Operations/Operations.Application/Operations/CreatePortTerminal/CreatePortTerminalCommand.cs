using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.CreatePortTerminal;

public sealed record CreatePortTerminalCommand(
    Guid PortId,
    string Name,
    string? Code = null,
    string? ArabicName = null) : IRequest<Result<Guid>>;
