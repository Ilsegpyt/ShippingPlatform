using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.UpdatePortTerminal;

public sealed record UpdatePortTerminalCommand(
    Guid Id,
    Guid PortId,
    string Name,
    string? Code,
    string? ArabicName,
    bool IsActive) : IRequest<Result<bool>>;
