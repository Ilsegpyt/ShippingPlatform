using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using Customers.Application.Queries.GetCustomerById;
using Identity.Contracts;
using MediatR;

namespace Customers.Application.Queries.ListCustomers;

public sealed record ListCustomersQuery(
    PaginationRequest Pagination,
    Guid UserId)
    : IRequest<Result<PagedResult<CustomerResponse>>>;

public sealed class ListCustomersQueryHandler(
    ICustomerRepository repository,
    IAccountManagerQueries accountManagerQueries,
    IUserAccessQueries userAccessQueries)
    : IRequestHandler<ListCustomersQuery, Result<PagedResult<CustomerResponse>>>
{
    public async Task<Result<PagedResult<CustomerResponse>>> Handle(
        ListCustomersQuery query,
        CancellationToken ct)
    {
        var page = query.Pagination.PageNumber;
        var pageSize = query.Pagination.PageSize;
        var skip = (page - 1) * pageSize;

        var userAccess =
            await userAccessQueries.GetAccessInfoAsync(
                query.UserId,
                ct);

        var isAccountManager =
            userAccess is not null &&
            userAccess.IsActive &&
            userAccess.TokenType == "internal" &&
            userAccess.RoleName == "Account Manager";

        if (isAccountManager)
        {
            var assignedCustomerIds =
                await accountManagerQueries.GetAssignedCustomerIdsAsync(
                    query.UserId,
                    ct);

            var totalCount =
                await repository.CountByIdsAsync(
                    assignedCustomerIds,
                    ct);

            var customers =
                await repository.ListByIdsAsync(
                    assignedCustomerIds,
                    skip,
                    pageSize,
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

            return Result.Success(
                new PagedResult<CustomerResponse>(
                    items,
                    totalCount,
                    page,
                    pageSize));
        }

        var allCustomers =
            await repository.ListAsync(
                skip,
                pageSize,
                ct);

        var allTotalCount =
            await repository.CountAsync(ct);

        var allItems = allCustomers
            .Select(c => new CustomerResponse(
                c.Id,
                c.OwnerName,
                c.CompanyName,
                c.OwnerPhone,
                c.OwnerEmail,
                c.Industry,
                c.Status.ToString()))
            .ToList();

        return Result.Success(
            new PagedResult<CustomerResponse>(
                allItems,
                allTotalCount,
                page,
                pageSize));
    }
}