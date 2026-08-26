using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using MediatR;

namespace Customers.Application.Queries;

public sealed record CustomerResponse(Guid Id, string OwnerName, string CompanyName,
    string OwnerPhone, string OwnerEmail, string? Industry, string Status);

public sealed record GetCustomerByIdQuery(Guid CustomerId) : IRequest<Result<CustomerResponse>>;

public sealed class GetCustomerByIdQueryHandler(ICustomerRepository repository)
    : IRequestHandler<GetCustomerByIdQuery, Result<CustomerResponse>>
{
    public async Task<Result<CustomerResponse>> Handle(GetCustomerByIdQuery query, CancellationToken ct)
    {
        var customer = await repository.GetByIdAsync(query.CustomerId, ct);
        if (customer is null) return Result.Failure<CustomerResponse>("Customer not found.");

        return Result.Success(new CustomerResponse(
            customer.Id, customer.OwnerName, customer.CompanyName,
            customer.OwnerPhone, customer.OwnerEmail, customer.Industry, customer.Status.ToString()));
    }
}