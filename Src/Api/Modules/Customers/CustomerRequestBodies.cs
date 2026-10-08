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

public sealed record DeleteCustomersRequest(
    IReadOnlyCollection<Guid> CustomerIds);

public sealed record CreateCustomerVoiceRequest(
    Guid ShipmentId,
    string Subject,
    string Message);

