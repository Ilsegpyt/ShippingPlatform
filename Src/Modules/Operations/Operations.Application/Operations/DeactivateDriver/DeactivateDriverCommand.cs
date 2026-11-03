using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.DeactivateDriver;

public sealed record DeactivateDriverCommand(Guid Id) : IRequest<Result<bool>>;
