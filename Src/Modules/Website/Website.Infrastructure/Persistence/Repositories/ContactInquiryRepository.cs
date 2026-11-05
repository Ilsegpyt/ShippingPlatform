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
        string? search = null,
        CancellationToken ct = default)
    {
        var query = dbContext.ContactInquiries
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(x =>
                EF.Functions.Like(x.FullName, $"%{s}%") ||
                EF.Functions.Like(x.Company, $"%{s}%") ||
                EF.Functions.Like(x.Email, $"%{s}%") ||
                EF.Functions.Like(x.Phone, $"%{s}%") ||
                EF.Functions.Like(x.Service, $"%{s}%"));
        }

        query = query.OrderByDescending(x => x.CreatedAtUtc);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<ContactInquiry?> GetByIdAsync(
        int id,
        CancellationToken ct = default)
    {
        return await dbContext.ContactInquiries
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}
