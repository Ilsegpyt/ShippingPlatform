using BuildingBlocks.Application.Contracts;
using MediatR;
using Reports.Application.Abstractions;

namespace Reports.Application.Reports.DownloadReport;

public sealed class DownloadReportQueryHandler(
    ISubAccountQueries subAccountQueries,
    IReportRepository reportRepository)
    : IRequestHandler<DownloadReportQuery, DownloadReportResult?>
{
    public async Task<DownloadReportResult?> Handle(
        DownloadReportQuery request,
        CancellationToken ct)
    {
        var access =
            await subAccountQueries.GetAccessInfoAsync(
                request.UserId,
                ct);

        if (access is null || !access.IsActive)
            return null;

        if (!access.Permissions.Contains("reports.view"))
            return null;

        var report =
            await reportRepository.GetByIdAsync(
                request.ReportId,
                ct);

        if (report is null)
            return null;

        if (report.CustomerId != access.OrganizationId)
            return null;

        if (!access.HasFullScope &&
            !access.Scopes.Any(scope =>
                scope.Category == (int)report.Category &&
                MatchesService(
                    scope.Service,
                    (int)report.Service) &&
                MatchesShipmentType(
                    scope.ShipmentType,
                    (int)report.ShipmentType)))
        {
            return null;
        }

        return new DownloadReportResult(
            report.FileName,
            report.StorageKey);
    }

    private static bool MatchesService(
        int scopeService,
        int reportService)
    {
        if (scopeService == 4)
            return reportService is 2 or 3;

        return scopeService == reportService;
    }

    private static bool MatchesShipmentType(
        int scopeType,
        int reportType)
    {
        if (scopeType == 1)
            return reportType is 2 or 3;

        return scopeType == reportType;
    }
}