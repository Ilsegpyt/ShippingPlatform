using BuildingBlocks.Application;
using MediatR;

namespace Customers.Application.Commands.DeleteCustomer;

public sealed record DeleteCustomersCommand(
    IReadOnlyCollection<Guid> CustomerIds,
    Guid DeletedByUserId) : IRequest<Result>;