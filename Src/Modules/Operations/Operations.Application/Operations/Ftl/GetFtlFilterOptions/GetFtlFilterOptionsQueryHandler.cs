using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.Ftl.GetFtlFilterOptions;

public sealed class GetFtlFilterOptionsQueryHandler(
    IFtlOperationRepository repository)
    : IRequestHandler<GetFtlFilterOptionsQuery, Result<FtlFilterOptionsDto>>
{
    public async Task<Result<FtlFilterOptionsDto>> Handle(
        GetFtlFilterOptionsQuery query,
        CancellationToken ct)
    {
        var options = await repository.GetFilterOptionsAsync(
            query.OperationType,
            query.Status,
            query.OnlyActive,
            query.IsDeleted,
            ct);

        return Result.Success(options);
    }
}
