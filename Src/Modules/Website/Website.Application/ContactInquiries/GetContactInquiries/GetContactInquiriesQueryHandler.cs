using BuildingBlocks.Application;
using MediatR;
using Website.Application.Abstractions.Repositories;

namespace Website.Application.ContactInquiries.GetContactInquiries;

public sealed class GetContactInquiriesQueryHandler(
    IContactInquiryRepository contactInquiryRepository)
    : IRequestHandler<
        GetContactInquiriesQuery,
        Result<PagedResult<Domain.Entities.ContactInquiry>>>
{
    public async Task<Result<PagedResult<Domain.Entities.ContactInquiry>>> Handle(
        GetContactInquiriesQuery query,
        CancellationToken ct)
    {
        var pageNumber = query.Pagination.PageNumber;
        var pageSize = query.Pagination.PageSize;

        var (items, totalCount) =
            await contactInquiryRepository.GetPagedAsync(
                pageNumber,
                pageSize,
                ct);

        var result =
            new PagedResult<Domain.Entities.ContactInquiry>(
                items,
                totalCount,
                pageNumber,
                pageSize);

        return Result.Success(result);
    }
}