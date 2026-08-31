using BuildingBlocks.Application;
using MediatR;

namespace Identity.Application.AccountManagerAssignments.RemoveAccountManager;

public sealed record RemoveAccountManagerCommand(
    Guid CustomerId)
    : IRequest<Result>;
