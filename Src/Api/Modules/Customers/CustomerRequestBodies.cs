using Customers.Application.Queries;

namespace Api.Modules.Customers;

public sealed record UpdateCustomerProfileRequest(
    string OwnerName,
    string CompanyName,
    string OwnerPhone,
    string? Industry);

public sealed record RegisterCustomerRequest(
    string OwnerName,
    string CompanyName,
    string OwnerPhone,
    string OwnerEmail,
    string? Industry);
public sealed record UpdateCustomerEmailRequest(
    string Email);
public sealed record SearchCustomerMultiSchedulesRequest(
    IReadOnlyList<SearchCustomerMultiRouteItem> Routes);