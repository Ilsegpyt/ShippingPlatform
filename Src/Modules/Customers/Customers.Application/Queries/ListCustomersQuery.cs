using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using MediatR;

namespace Customers.Application.Queries;

/// <summary>Default listing — relies on CustomersDbContext's global query filter
/// (!IsDeleted), so soft-deleted customers never appear here.</summary>
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

/// <summary>Admin-only listing that bypasses the soft-delete filter.
/// IncludeDeleted = true  → everyone (active + deleted)
/// IncludeDeleted = false → deleted only</summary>
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
