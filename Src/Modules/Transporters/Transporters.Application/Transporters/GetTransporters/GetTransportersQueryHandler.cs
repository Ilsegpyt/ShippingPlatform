using BuildingBlocks.Application;
using MediatR;
using Transporters.Application.Abstractions.Repositories;
using Transporters.Domain.Entities;

namespace Transporters.Application.Transporters.GetTransporters;

public sealed class GetTransportersQueryHandler(
    ITransporterRepository transporterRepository)
    : IRequestHandler<
        GetTransportersQuery,
        Result<PagedResult<Transporter>>>
{
    public async Task<Result<PagedResult<Transporter>>> Handle(
        GetTransportersQuery query,
        CancellationToken ct)
    {
        var pageNumber = query.Pagination.PageNumber;
        var pageSize = query.Pagination.PageSize;
        var search = query.Pagination.Search;

        var (items, totalCount) =
            await transporterRepository.GetPagedAsync(
                pageNumber,
                pageSize,
                search,
                ct);

        var result = new PagedResult<Transporter>(
            items,
            totalCount,
            pageNumber,
            pageSize);

        return Result.Success(result);
    }
}
