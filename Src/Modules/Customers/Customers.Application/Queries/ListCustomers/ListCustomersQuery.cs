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
public sealed record ListCustomersQuery(PaginationRequest Pagination) : IRequest<Result<PagedResult<CustomerResponse>>>;


public sealed class ListCustomersQueryHandler(ICustomerRepository repository)
    : IRequestHandler<ListCustomersQuery, Result<PagedResult<CustomerResponse>>>
{
    public async Task<Result<PagedResult<CustomerResponse>>> Handle(
        ListCustomersQuery query,
        CancellationToken ct)
    {
        var page = query.Pagination.PageNumber;
        var pageSize = query.Pagination.PageSize;

        var skip = (page - 1) * pageSize;

        var totalCount = await repository.CountAsync(ct);

        var customers = await repository.ListAsync(skip, pageSize, ct);

        var items = customers
            .Select(c => new CustomerResponse(
                c.Id,
                c.OwnerName,
                c.CompanyName,
                c.OwnerPhone,
                c.OwnerEmail,
                c.Industry,
                c.Status.ToString()))
            .ToList();

        var result = new PagedResult<CustomerResponse>(
            items,
            totalCount,
            page,
            pageSize);

        return Result.Success(result);
    }
}