using BuildingBlocks.Application;
using Customers.Contracts;
using Identity.Contracts;
using MediatR;
using Shipments.Application.Abstractions;
using Shipments.Domain.Shipments;

namespace Shipments.Application.Shipments.GetShipmentExceptions;

public sealed class GetShipmentExceptionsQueryHandler(
    IShipmentRepository shipmentRepository,
    IAccountManagerQueries accountManagerQueries,
    IUserAccessQueries userAccessQueries,
    ICustomerQueries customerQueries)
    : IRequestHandler<
        GetShipmentExceptionsQuery,
        Result<GetShipmentExceptionsResponse>>
{
    public async Task<Result<GetShipmentExceptionsResponse>> Handle(
        GetShipmentExceptionsQuery query,
        CancellationToken ct)
    {
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

            if (assignedCustomerIds.Count == 0)
            {
                return new GetShipmentExceptionsResponse(
                    0,
                    0,
                    0);
            }

            var missingDocuments =
                await CountByCustomerIdsAndStatusAsync(
                    assignedCustomerIds,
                    ShipmentStatus.MissingDocs,
                    ct);

            var cancelled =
                await CountByCustomerIdsAndStatusAsync(
                    assignedCustomerIds,
                    ShipmentStatus.Cancelled,
                    ct);

            return new GetShipmentExceptionsResponse(
                missingDocuments + cancelled,
                missingDocuments,
                cancelled);
        }

        if (
            query.TokenType == "impersonation" &&
            Guid.TryParse(
                query.OrganizationId,
                out var impersonatedCustomerId))
        {
            var customer =
                await customerQueries.GetByIdAsync(
                    impersonatedCustomerId,
                    ct);

            if (customer is null || !customer.IsActive)
            {
                return new GetShipmentExceptionsResponse(
                    0,
                    0,
                    0);
            }

            var missingDocuments =
                await CountByCustomerIdAndStatusAsync(
                    impersonatedCustomerId,
                    ShipmentStatus.MissingDocs,
                    ct);

            var cancelled =
                await CountByCustomerIdAndStatusAsync(
                    impersonatedCustomerId,
                    ShipmentStatus.Cancelled,
                    ct);

            return new GetShipmentExceptionsResponse(
                missingDocuments + cancelled,
                missingDocuments,
                cancelled);
        }

        var currentCustomer =
            await customerQueries.GetByUserIdAsync(
                query.UserId,
                ct);

        if (currentCustomer is not null && currentCustomer.IsActive)
        {
            var missingDocuments =
                await CountByCustomerIdAndStatusAsync(
                    currentCustomer.CustomerId,
                    ShipmentStatus.MissingDocs,
                    ct);

            var cancelled =
                await CountByCustomerIdAndStatusAsync(
                    currentCustomer.CustomerId,
                    ShipmentStatus.Cancelled,
                    ct);

            return new GetShipmentExceptionsResponse(
                missingDocuments + cancelled,
                missingDocuments,
                cancelled);
        }

        var allMissingDocuments =
            await shipmentRepository.CountByStatusAsync(
                ShipmentStatus.MissingDocs,
                ct);

        var allCancelled =
            await shipmentRepository.CountByStatusAsync(
                ShipmentStatus.Cancelled,
                ct);

        return new GetShipmentExceptionsResponse(
            allMissingDocuments + allCancelled,
            allMissingDocuments,
            allCancelled);
    }

    private async Task<int> CountByCustomerIdAndStatusAsync(
        Guid customerId,
        ShipmentStatus status,
        CancellationToken ct)
    {
        var shipments =
            await shipmentRepository.GetByCustomerIdAsync(
                customerId,
                ct);

        return shipments.Count(x => x.Status == status);
    }

    private async Task<int> CountByCustomerIdsAndStatusAsync(
        IReadOnlyCollection<Guid> customerIds,
        ShipmentStatus status,
        CancellationToken ct)
    {
        var shipments =
            await shipmentRepository.GetByCustomerIdsAsync(
                customerIds,
                0,
                int.MaxValue,
                ct);

        return shipments.Count(x => x.Status == status);
    }
}