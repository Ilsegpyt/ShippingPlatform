using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.DeactivateFtlDriver;

public sealed record DeactivateFtlDriverCommand(Guid Id) : IRequest<Result<bool>>;
