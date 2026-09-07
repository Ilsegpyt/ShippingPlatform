using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using Customers.Application.Queries.GetCustomerById;
using MediatR;

namespace Customers.Application.Queries.ListCustomers;

/// <summary>
/// Lists non-deleted customers.
/// Relies on the global query filter (!IsDeleted),
/// so soft-deleted customers are excluded from the result.
/// </summary>
public sealed record ListCustomersQuery : IRequest<Result<IReadOnlyList<CustomerResponse>>>;

public sealed class ListCustomersQueryHandler(ICustomerRepository repository)
    : IRequestHandler<ListCustomersQuery, Result<IReadOnlyList<CustomerResponse>>>
{
    public async Task<Result<IReadOnlyList<CustomerResponse>>> Handle(ListCustomersQuery query, CancellationToken ct)
    {
        var customers = await repository.ListAsync(ct); 
        var response = customers.Select(c => new CustomerResponse(
            c.Id, c.OwnerName, c.CompanyName, c.OwnerPhone, c.OwnerEmail, c.Industry, c.Status.ToString())).ToList();

        return Result.Success<IReadOnlyList<CustomerResponse>>(response);
    }
}

