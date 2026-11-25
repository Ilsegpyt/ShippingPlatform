using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Operations.Ftl.Common;

namespace Operations.Application.Operations.Ftl.GetAllFtlOperations;

public sealed record GetAllFtlOperationsQuery(
    string? OperationType = null,
    bool OnlyActive = false) : IRequest<Result<FtlWorkspaceDto>>;

