using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.UpdateFtlTransporter;

public sealed record UpdateFtlTransporterCommand(Guid Id, string Name, bool IsActive) : IRequest<Result<bool>>;
