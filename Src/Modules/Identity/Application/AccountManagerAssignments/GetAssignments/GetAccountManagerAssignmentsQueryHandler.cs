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

        var internalUsers =
            await internalUserRepository.GetByIdsAsync(
                accountManagerIds,
                ct);

        var customers =
            await customerQueries.GetByIdsAsync(
                customerIds,
                ct);

        var internalUsersById = internalUsers
            .ToDictionary(x => x.Id);

        var customersById = customers
            .ToDictionary(x => x.CustomerId);

        var result = new List<AccountManagerAssignmentResponse>();

        foreach (var accountManagerId in accountManagerIds)
        {
            if (!internalUsersById.TryGetValue(
                    accountManagerId,
                    out var accountManager))
            {
                continue;
            }

            var managerCustomers = assignments
                .Where(x => x.AccountManagerId == accountManagerId)
                .Select(x =>
                {
                    customersById.TryGetValue(
                        x.CustomerId,
                        out var customer);

                    return customer;
                })
                .Where(x => x is not null)
                .Select(customer =>
                    new AccountManagerCustomerResponse(
                        customer!.CustomerId,
                        customer.OwnerName,
                        customer.CompanyName,
                        customer.IsActive,
                        customer.IsDeleted))
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