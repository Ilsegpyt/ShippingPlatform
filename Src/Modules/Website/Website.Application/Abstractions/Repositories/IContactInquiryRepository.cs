using Website.Domain.Entities;

namespace Website.Application.Abstractions.Repositories;

public interface IContactInquiryRepository
{
    void Add(ContactInquiry inquiry);

    Task<(IReadOnlyList<ContactInquiry> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        CancellationToken ct = default);

    Task<ContactInquiry?> GetByIdAsync(
        int id,
        CancellationToken ct = default);
}
