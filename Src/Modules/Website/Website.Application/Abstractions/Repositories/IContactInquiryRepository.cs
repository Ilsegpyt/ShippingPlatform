using Website.Domain.Entities;

namespace Website.Application.Abstractions.Repositories;

public interface IContactInquiryRepository
{
    void Add(ContactInquiry inquiry);

    Task<(IReadOnlyList<ContactInquiry> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct);

    Task<ContactInquiry?> GetByIdAsync(
        int id,
        CancellationToken ct);
}