using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.CreateFtlTransporter;

public sealed record CreateFtlTransporterCommand(string Name) : IRequest<Result<Guid>>;
