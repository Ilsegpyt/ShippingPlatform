using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Operations.GetAllOperations;

namespace Operations.Application.Operations.GetDeletedOperations;

public sealed record GetDeletedOperationsQuery
    : IRequest<Result<IReadOnlyList<OperationDto>>>;
