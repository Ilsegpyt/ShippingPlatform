
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
public sealed record ListAllCustomersQuery(bool DeletedOnly , PaginationRequest Pagination) : IRequest<Result<PagedResult<CustomerResponse>>>;

public sealed class ListAllCustomersQueryHandler(ICustomerRepository repository)
    : IRequestHandler<ListAllCustomersQuery, Result<PagedResult<CustomerResponse>>>
{
    public async Task<Result<PagedResult<CustomerResponse>>> Handle(
        ListAllCustomersQuery query,
        CancellationToken ct)
    {
        var page = query.Pagination.PageNumber;
        var pageSize = query.Pagination.PageSize;

        var skip = (page - 1) * pageSize;

        var customers = await repository.ListIgnoringDeletedFilterAsync(
            query.DeletedOnly,
            skip,
            pageSize,
            ct);

        var totalCount = await repository.CountIgnoringDeletedFilterAsync(
            query.DeletedOnly,
            ct);

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