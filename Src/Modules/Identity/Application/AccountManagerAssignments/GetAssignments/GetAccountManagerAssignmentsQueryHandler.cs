using BuildingBlocks.Application;
using Customers.Contracts;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.AccountManagerAssignments.GetAssignments;

public sealed class GetAccountManagerAssignmentsQueryHandler(
    IAccountManagerAssignmentRepository assignmentRepository,
    IInternalUserRepository internalUserRepository,
    ICustomerQueries customerQueries)
    : IRequestHandler<
        GetAccountManagerAssignmentsQuery,
        Result<IReadOnlyList<AccountManagerAssignmentResponse>>>
{
    public async Task<Result<IReadOnlyList<AccountManagerAssignmentResponse>>> Handle(
        GetAccountManagerAssignmentsQuery query,
        CancellationToken ct)
    {
        var assignments =
            await assignmentRepository.ListAllAsync(ct);

        if (assignments.Count == 0)
            return Array.Empty<AccountManagerAssignmentResponse>();

        var accountManagerIds = assignments
            .Select(x => x.AccountManagerId)
            .Distinct()
            .ToList();

        var customerIds = assignments
            .Select(x => x.CustomerId)
            .Distinct()
            .ToList();

        var internalUsers = new List<Identity.Domain.Entities.InternalUser>();

        foreach (var accountManagerId in accountManagerIds)
        {
            var internalUser = await internalUserRepository.GetByIdAsync(
                accountManagerId,
                ct);

            if (internalUser is not null)
                internalUsers.Add(internalUser);
        }

        var customers = await customerQueries.GetByIdsAsync(
            customerIds,
            ct);

        var customersById = customers.ToDictionary(x => x.CustomerId);

        var result = new List<AccountManagerAssignmentResponse>();

        foreach (var accountManager in internalUsers)
        {
            var managerAssignments = assignments
                .Where(x => x.AccountManagerId == accountManager.Id)
                .ToList();

            var managerCustomers = managerAssignments
                .Where(x => customersById.ContainsKey(x.CustomerId))
                .Select(x =>
                {
                    var customer = customersById[x.CustomerId];

                    return new AccountManagerCustomerResponse(
                        customer.CustomerId,
                        customer.OwnerName,
                        customer.CompanyName,
                        customer.IsActive,
                        customer.IsDeleted);
                })
                .ToList();

            result.Add(
                new AccountManagerAssignmentResponse(
                    accountManager.Id,
                    accountManager.Name,
                    accountManager.Email,
                    managerCustomers));
        }

        return result;
    }
}