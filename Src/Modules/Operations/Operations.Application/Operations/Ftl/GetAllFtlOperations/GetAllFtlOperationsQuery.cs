using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Operations.Ftl.Common;

namespace Operations.Application.Operations.Ftl.GetAllFtlOperations;

public sealed record GetAllFtlOperationsQuery : IRequest<Result<FtlWorkspaceDto>>;

