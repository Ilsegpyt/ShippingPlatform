using BuildingBlocks.Application;
using MediatR;

namespace Customers.Application.Customers.DeleteCustomer;

public sealed record DeleteCustomerCommand(
    Guid CustomerId,
    Guid DeletedByUserId) : IRequest<Result>;