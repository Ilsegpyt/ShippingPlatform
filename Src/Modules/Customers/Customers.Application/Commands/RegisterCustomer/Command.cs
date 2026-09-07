using BuildingBlocks.Application;
using MediatR;

namespace Customers.Application.Customers.RegisterCustomer;

public sealed record RegisterCustomerCommand(string OwnerName, string CompanyName, string OwnerPhone, string OwnerEmail, string? Industry)
    : IRequest<Result<RegisterCustomerResponse>>;

public sealed record RegisterCustomerResponse(Guid CustomerId, string TemporaryPassword);