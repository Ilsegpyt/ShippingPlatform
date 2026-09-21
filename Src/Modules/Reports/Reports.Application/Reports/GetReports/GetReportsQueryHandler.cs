using Customers.Contracts;
using Identity.Contracts;
using MediatR;
using Reports.Application.Abstractions;
using Reports.Domain.Entities;

namespace Reports.Application.Reports.GetReports;

public sealed class GetReportsQueryHandler(
    ISubAccountQueries subAccountQueries,
    IUserAccessQueries userAccessQueries,
    ICustomerQueries customerQueries,
    IAccountManagerQueries accountManagerQueries,
    IReportRepository reportRepository)
    : IRequestHandler<GetReportsQuery, IReadOnlyList<Report>>
{
    public async Task<IReadOnlyList<Report>> Handle(
        GetReportsQuery request,
        CancellationToken ct)
    {
        // 1. Check InternalUser / SuperAdmin
        var userAccess =
            await userAccessQueries.GetAccessInfoAsync(
                request.UserId,
                ct);

        if (userAccess is not null &&
            userAccess.IsActive &&
            userAccess.TokenType == "internal" &&
            userAccess.RoleName == "Super Admin")
        {
            return await reportRepository.GetAllAsync(ct);
        }

        // 2. Check Account Manager
        var isAccountManager =
            userAccess is not null &&
            userAccess.IsActive &&
            userAccess.TokenType == "internal" &&
            userAccess.RoleName == "Account Manager";

        if (isAccountManager)
        {
            var assignedCustomerIds =
                await accountManagerQueries.GetAssignedCustomerIdsAsync(
                    request.UserId,
                    ct);

            if (assignedCustomerIds.Count == 0)
                return [];

            return await reportRepository.GetByCustomerIdsAsync(
                assignedCustomerIds,
                ct);
        }

        // 3. Check Customer
        var customerAccess =
            await customerQueries.GetByUserIdAsync(
                request.UserId,
                ct);

        if (customerAccess is not null)
        {
            if (!customerAccess.IsActive)
                return [];

            return await reportRepository.GetByCustomerIdAsync(
                customerAccess.CustomerId,
                ct);
        }

        // 4. Check SubAccount
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

        // 5. SubAccount with Full Scope
        if (access.HasFullScope)
            return reports;

        // 6. SubAccount with Custom Scope
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