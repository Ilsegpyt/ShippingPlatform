using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.UpdatePort;

public sealed record UpdatePortCommand(Guid Id, string Name, string? Code, bool IsActive) : IRequest<Result<bool>>;
