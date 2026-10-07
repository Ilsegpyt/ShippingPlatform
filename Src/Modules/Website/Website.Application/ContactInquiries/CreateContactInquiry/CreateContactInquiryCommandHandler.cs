using BuildingBlocks.Application;
using MediatR;
using Website.Application.Abstractions;
using Website.Application.Abstractions.Repositories;
using Website.Domain.Entities;

namespace Website.Application.ContactInquiries.CreateContactInquiry;

public sealed class CreateContactInquiryCommandHandler(
    IContactInquiryRepository contactInquiryRepository,
    IWebsiteUnitOfWork websiteUnitOfWork)
    : IRequestHandler<CreateContactInquiryCommand, Result>
{
    public async Task<Result> Handle(
        CreateContactInquiryCommand cmd,
        CancellationToken ct)
    {
        var inquiry = new ContactInquiry
        {
            FullName = cmd.FullName,
            Company = cmd.Company,
            Email = cmd.Email,
            Phone = cmd.Phone,
            Service = cmd.Service,
            Message = cmd.Message,
            CreatedAtUtc = DateTime.UtcNow
        };

        contactInquiryRepository.Add(inquiry);

        await websiteUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}