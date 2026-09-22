using BuildingBlocks.Application;
using Customers.Contracts;
using MediatR;

namespace Identity.Application.AccountManagerAssignments.GetUnassignedCustomers;

public sealed record GetUnassignedCustomersQuery
    : IRequest<Result<IReadOnlyList<CustomerAssignmentInfo>>>;