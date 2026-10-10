
using BuildingBlocks.Application;
using MediatR;

namespace Customers.Application.Commands.UpdateCustomerProfile;

public sealed record UpdateCustomerProfileCommand(
    Guid CustomerId, string OwnerName, string CompanyName, string OwnerPhone, string? Industry)
    : IRequest<Result>;
