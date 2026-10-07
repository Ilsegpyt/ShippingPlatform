using BuildingBlocks.Application;
using MediatR;
using Website.Domain.Entities;

namespace Website.Application.ContactInquiries.GetContactInquiries;

public sealed record GetContactInquiriesQuery(
    PaginationRequest Pagination)
    : IRequest<Result<PagedResult<ContactInquiry>>>;