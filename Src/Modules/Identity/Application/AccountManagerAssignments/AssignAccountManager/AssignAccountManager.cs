using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.AccountManagerAssignments.AssignAccountManager;

public sealed record AssignAccountManagerCommand(
    Guid AccountManagerId,
    Guid CustomerId)
    : IRequest<Result>;