using BuildingBlocks.Application;
using MediatR;
using Website.Domain.Entities;

namespace Website.Application.ContactInquiries.GetContactInquiryById;

public sealed record GetContactInquiryByIdQuery(
    int Id)
    : IRequest<Result<ContactInquiry>>;