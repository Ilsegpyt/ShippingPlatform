using BuildingBlocks.Application;
using MediatR;

namespace Customers.Application.Queries.GetCustomerById;

public sealed record CustomerResponse(Guid Id, string OwnerName, string CompanyName,
    string OwnerPhone, string OwnerEmail, string? Industry, string Status);

public sealed record GetCustomerByIdQuery(Guid CustomerId) : IRequest<Result<CustomerResponse>>;

