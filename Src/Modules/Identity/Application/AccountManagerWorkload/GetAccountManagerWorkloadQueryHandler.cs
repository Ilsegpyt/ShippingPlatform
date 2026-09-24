using BuildingBlocks.Application;
using Identity.Domain.Repositories;
using MediatR;
using Shipments.Contracts;

namespace Identity.Application.AccountManagerWorkload;

public sealed class GetAccountManagerWorkloadQueryHandler(
    IAccountManagerAssignmentRepository assignmentRepository,
    IInternalUserRepository internalUserRepository,
    IShipmentQueryService shipmentQueryService)
    : IRequestHandler<
        GetAccountManagerWorkloadQuery,
        Result<GetAccountManagerWorkloadResponse>>
{
    public async Task<Result<GetAccountManagerWorkloadResponse>> Handle(
        GetAccountManagerWorkloadQuery query,
        CancellationToken ct)
    {
        var assignments =
            await assignmentRepository.ListAllAsync(ct);

        if (assignments.Count == 0)
        {
            return Result.Success(
                new GetAccountManagerWorkloadResponse(
                    []));
        }

        var accountManagerIds = assignments
            .Select(x => x.AccountManagerId)
            .Distinct()
            .ToList();

        var internalUsers =
            await internalUserRepository.GetByIdsAsync(
                accountManagerIds,
                ct);

        var items = new List<AccountManagerWorkloadItem>();

        foreach (var accountManagerId in accountManagerIds)
        {
            var accountManager =
                internalUsers.FirstOrDefault(
                    x => x.Id == accountManagerId);

            if (accountManager is null)
                continue;

            var customerIds = assignments
                .Where(x => x.AccountManagerId == accountManagerId)
                .Select(x => x.CustomerId)
                .Distinct()
                .ToList();

            var activeShipments =
                await shipmentQueryService.CountActiveByCustomerIdsAsync(
                    customerIds,
                    ct);

            items.Add(
                new AccountManagerWorkloadItem(
                    accountManagerId,
                    accountManager.Name,
                    customerIds.Count,
                    activeShipments));
        }

        return Result.Success(
            new GetAccountManagerWorkloadResponse(items));
    }
}