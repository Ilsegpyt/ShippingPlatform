using BuildingBlocks.Application;
using MediatR;

namespace Website.Application.ContactInquiries.CreateContactInquiry;

public sealed record CreateContactInquiryCommand(
    string FullName,
    string Company,
    string Email,
    string Phone,
    string Service,
    string Message)
    : IRequest<Result>;