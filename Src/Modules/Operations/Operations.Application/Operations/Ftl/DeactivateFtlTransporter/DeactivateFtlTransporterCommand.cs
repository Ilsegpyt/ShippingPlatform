using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.DeactivateFtlTransporter;

public sealed record DeactivateFtlTransporterCommand(Guid Id) : IRequest<Result<bool>>;
