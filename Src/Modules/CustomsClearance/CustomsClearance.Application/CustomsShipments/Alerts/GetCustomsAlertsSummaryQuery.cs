using BuildingBlocks.Application;
using CustomsClearance.Application.Abstractions.Repositories;
using CustomsClearance.Domain.Enums;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.Alerts;

// Alert counts over all Open Ocean Import shipments (not just the current page).
public sealed record GetCustomsAlertsSummaryQuery
    : IRequest<Result<CustomsAlertsSummary>>;

public sealed class GetCustomsAlertsSummaryQueryHandler(
    ICustomsShipmentRepository customsShipmentRepository,
    TimeProvider clock)
    : IRequestHandler<GetCustomsAlertsSummaryQuery, Result<CustomsAlertsSummary>>
{
    public async Task<Result<CustomsAlertsSummary>> Handle(
        GetCustomsAlertsSummaryQuery query,
        CancellationToken ct)
    {
        var today = CustomsAlerts.CairoToday(clock);

        // Sequential: one DbContext per request does not allow parallel queries.
        Task<int> Count(CustomsAlertFilter filter) =>
            customsShipmentRepository.CountAsync(
                ReportType.OceanImport,
                CustomsShipmentStatus.Open,
                CustomsAlerts.Predicate(filter, today),
                ct);

        var summary = new CustomsAlertsSummary(
            today,
            EtaSoon: await Count(CustomsAlertFilter.EtaSoon),
            EtaOverdue: await Count(CustomsAlertFilter.EtaOverdue),
            FreeTimeSoon: await Count(CustomsAlertFilter.FreeTimeSoon),
            FreeTimeExpired: await Count(CustomsAlertFilter.FreeTimeExpired),
            Total: await Count(CustomsAlertFilter.Any));

        return Result.Success(summary);
    }
}
