using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.AccountManagerAssignments.ChangeAccountManager;

public sealed record ChangeAccountManagerCommand(
    Guid CustomerId,
    Guid NewAccountManagerId)
    : IRequest<Result>;