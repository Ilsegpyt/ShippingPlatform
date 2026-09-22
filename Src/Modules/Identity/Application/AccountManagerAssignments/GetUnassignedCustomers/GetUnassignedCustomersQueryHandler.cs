using BuildingBlocks.Application;
using Customers.Contracts;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.AccountManagerAssignments.GetUnassignedCustomers;

public sealed class GetUnassignedCustomersQueryHandler(
    IAccountManagerAssignmentRepository assignmentRepository,
    ICustomerQueries customerQueries)
    : IRequestHandler<
        GetUnassignedCustomersQuery,
        Result<IReadOnlyList<CustomerAssignmentInfo>>>
{
    public async Task<Result<IReadOnlyList<CustomerAssignmentInfo>>> Handle(
        GetUnassignedCustomersQuery query,
        CancellationToken ct)
    {
        var assignments =
            await assignmentRepository.ListAllAsync(ct);

        var assignedCustomerIds = assignments
            .Select(x => x.CustomerId)
            .ToHashSet();

        var allCustomers =
            await customerQueries.GetAllForAssignmentAsync(ct);

        var unassignedCustomers = allCustomers
            .Where(customer =>
                !assignedCustomerIds.Contains(customer.CustomerId))
            .ToList();

        return unassignedCustomers;
    }
}