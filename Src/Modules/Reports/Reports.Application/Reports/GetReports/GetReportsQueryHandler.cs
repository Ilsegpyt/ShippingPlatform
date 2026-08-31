using BuildingBlocks.Application.Contracts;
using MediatR;
using Reports.Application.Abstractions;
using Reports.Domain.Report;

namespace Reports.Application.Reports.GetReports;

public sealed class GetReportsQueryHandler(
    ISubAccountQueries subAccountQueries,
    IReportRepository reportRepository)
    : IRequestHandler<GetReportsQuery, IReadOnlyList<Report>>
{
    public async Task<IReadOnlyList<Report>> Handle(
        GetReportsQuery request,
        CancellationToken ct)
    {
        var access =
            await subAccountQueries.GetAccessInfoAsync(
                request.UserId,
                ct);

        if (access is null || !access.IsActive)
            return [];

        if (!access.Permissions.Contains("reports.view"))
            return [];

        var reports =
            await reportRepository.GetByCustomerIdAsync(
                access.OrganizationId,
                ct);

        if (access.HasFullScope)
            return reports;

        return reports
            .Where(report => access.Scopes.Any(scope =>
                scope.Category == (int)report.Category &&
                MatchesService(
                    scope.Service,
                    (int)report.Service) &&
                MatchesShipmentType(
                    scope.ShipmentType,
                    (int)report.ShipmentType)))
            .ToList();
    }

    private static bool MatchesService(
        int scopeService,
        int reportService)
    {
        if (scopeService == 4) // Both
            return reportService is 2 or 3;

        return scopeService == reportService;
    }

    private static bool MatchesShipmentType(
        int scopeType,
        int reportType)
    {
        if (scopeType == 1) // All
            return reportType is 2 or 3;

        return scopeType == reportType;
    }
}