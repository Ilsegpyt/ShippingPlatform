using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.Ftl.GetFtlOperationsCounts;

public sealed record GetFtlOperationsCountsQuery : IRequest<Result<FtlOperationsCountsDto>>;
