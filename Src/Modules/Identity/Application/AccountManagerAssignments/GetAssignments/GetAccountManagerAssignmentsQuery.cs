using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.AccountManagerAssignments.GetAssignments;

public sealed record GetAccountManagerAssignmentsQuery
    : IRequest<Result<IReadOnlyList<AccountManagerAssignmentResponse>>>;

public sealed record AccountManagerAssignmentResponse(
    Guid AccountManagerId,
    string AccountManagerName,
    string AccountManagerEmail,
    IReadOnlyList<AccountManagerCustomerResponse> Customers);

public sealed record AccountManagerCustomerResponse(
    Guid CustomerId,
    string OwnerName,
    string CompanyName,
    bool IsActive,
    bool IsDeleted);