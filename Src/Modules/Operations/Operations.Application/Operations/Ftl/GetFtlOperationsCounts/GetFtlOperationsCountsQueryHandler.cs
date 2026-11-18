using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.Ftl.GetFtlOperationsCounts;

public sealed class GetFtlOperationsCountsQueryHandler(
    IFtlOperationRepository repository)
    : IRequestHandler<GetFtlOperationsCountsQuery, Result<FtlOperationsCountsDto>>
{
    public async Task<Result<FtlOperationsCountsDto>> Handle(
        GetFtlOperationsCountsQuery query,
        CancellationToken ct)
    {
        var counts = await repository.GetCountsAsync(ct);
        return Result.Success(counts);
    }
}
