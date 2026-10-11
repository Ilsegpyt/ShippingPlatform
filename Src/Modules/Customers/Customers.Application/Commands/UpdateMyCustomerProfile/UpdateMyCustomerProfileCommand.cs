using BuildingBlocks.Application;
using MediatR;

namespace Customers.Application.Commands.UpdateMyCustomerProfile;

public sealed record UpdateMyCustomerProfileCommand(
    Guid CustomerId,
    string OwnerName,
    string OwnerPhone
) : IRequest<Result>;