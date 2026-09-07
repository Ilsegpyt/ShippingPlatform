
using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using Customers.Application.Queries.GetCustomerById;
using MediatR;

namespace Customers.Application.Queries.ListAllCustomers;

/// <summary>
/// Lists customers while bypassing the soft-delete filter.
/// DeletedOnly = true  → deleted customers only
/// DeletedOnly = false → all customers (active + deleted)
/// </summary>
public sealed record ListAllCustomersQuery(bool DeletedOnly) : IRequest<Result<IReadOnlyList<CustomerResponse>>>;

public sealed class ListAllCustomersQueryHandler(ICustomerRepository repository)
    : IRequestHandler<ListAllCustomersQuery, Result<IReadOnlyList<CustomerResponse>>>
{
    public async Task<Result<IReadOnlyList<CustomerResponse>>> Handle(ListAllCustomersQuery query, CancellationToken ct)
    {
        var customers = await repository.ListIgnoringDeletedFilterAsync(query.DeletedOnly, ct);
        var response = customers.Select(c => new CustomerResponse(
            c.Id, c.OwnerName, c.CompanyName, c.OwnerPhone, c.OwnerEmail, c.Industry, c.Status.ToString())).ToList();

        return Result.Success<IReadOnlyList<CustomerResponse>>(response);
    }
}
