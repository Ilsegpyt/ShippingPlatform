using CustomsClearance.Application.CustomsShipments.Finance;
using CustomsClearance.Domain.Entities;

namespace CustomsClearance.Application.Abstractions.Repositories;

public interface ICustomsFinanceRepository
{
    void Add(CustomsFinanceRequest request);

    Task<CustomsFinanceRequest?> GetForUpdateAsync(Guid shipmentId, CancellationToken ct = default);

    // Active (not inactive), Open, Ocean Import.
    Task<bool> IsOpenOceanImportAsync(Guid shipmentId, CancellationToken ct = default);

    Task<(IReadOnlyList<CustomsFinanceRow> Items, int TotalCount)> GetPagedAsync(
        CustomsFinanceTab tab,
        DateOnly today,
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    Task<int> CountAsync(CustomsFinanceTab tab, DateOnly today, CancellationToken ct = default);
}
