using BuildingBlocks.Application;
using Customers.Contracts;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.AccountManagerAssignments.GetAssignments;

public sealed class GetAccountManagerAssignmentsQueryHandler(
    IAccountManagerAssignmentRepository assignmentRepository,
    IInternalUserRepository internalUserRepository,
    IRoleRepository roleRepository,
    ICustomerQueries customerQueries)
    : IRequestHandler<
        GetAccountManagerAssignmentsQuery,
        Result<IReadOnlyList<AccountManagerAssignmentResponse>>>
{
    public async Task<Result<IReadOnlyList<AccountManagerAssignmentResponse>>> Handle(
        GetAccountManagerAssignmentsQuery query,
        CancellationToken ct)
    {
        var accountManagerRole =
            await roleRepository.GetByNameAsync(
                "Account Manager",
                ct);

        if (accountManagerRole is null)
            return Array.Empty<AccountManagerAssignmentResponse>();

        var allInternalUsers =
            await internalUserRepository.GetAllAsync(
                0,
                int.MaxValue,
                ct);

        var accountManagers = allInternalUsers
            .Where(x => x.RoleId == accountManagerRole.Id)
            .ToList();

        if (accountManagers.Count == 0)
            return Array.Empty<AccountManagerAssignmentResponse>();

        var assignments =
            await assignmentRepository.ListAllAsync(ct);

        var customerIds = assignments
            .Select(x => x.CustomerId)
            .Distinct()
            .ToList();

        var customers = customerIds.Count > 0
            ? await customerQueries.GetByIdsAsync(
                customerIds,
                ct)
            : [];

        var customersById = customers
            .ToDictionary(x => x.CustomerId);

        var result = new List<AccountManagerAssignmentResponse>();

        foreach (var accountManager in accountManagers)
        {
            var managerCustomers = assignments
                .Where(x =>
                    x.AccountManagerId == accountManager.Id)
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