using BuildingBlocks.Application;
using MediatR;
using Website.Application.Abstractions.Repositories;
using Website.Domain.Entities;

namespace Website.Application.ContactInquiries.GetContactInquiryById;

public sealed class GetContactInquiryByIdQueryHandler(
    IContactInquiryRepository contactInquiryRepository)
    : IRequestHandler<
        GetContactInquiryByIdQuery,
        Result<ContactInquiry>>
{
    public async Task<Result<ContactInquiry>> Handle(
        GetContactInquiryByIdQuery query,
        CancellationToken ct)
    {
        var inquiry =
            await contactInquiryRepository.GetByIdAsync(
                query.Id,
                ct);

        if (inquiry is null)
            return Result.Failure<ContactInquiry>(
                "Contact inquiry was not found.");

        return Result.Success(inquiry);
    }
}