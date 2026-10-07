using Microsoft.EntityFrameworkCore;
using Website.Application.Abstractions.Repositories;
using Website.Domain.Entities;

namespace Website.Infrastructure.Persistence.Repositories;

public sealed class ContactInquiryRepository(
    WebsiteDbContext dbContext)
    : IContactInquiryRepository
{
    public void Add(ContactInquiry inquiry)
    {
        dbContext.ContactInquiries.Add(inquiry);
    }

    public async Task<(IReadOnlyList<ContactInquiry> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct)
    {
        var query = dbContext.ContactInquiries
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<ContactInquiry?> GetByIdAsync(
        int id,
        CancellationToken ct)
    {
        return await dbContext.ContactInquiries
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}