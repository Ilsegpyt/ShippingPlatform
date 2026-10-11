using BuildingBlocks.Application;
using MediatR;

namespace Customers.Application.Commands.UpdateCustomerEmail;

public sealed record UpdateCustomerEmail(Guid CustomerId, string Email) : IRequest<Result>;
