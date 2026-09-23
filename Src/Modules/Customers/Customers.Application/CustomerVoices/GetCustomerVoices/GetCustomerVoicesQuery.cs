using Customers.Application.Abstractions;
using Customers.Domain.Entities;
using MediatR;

namespace Customers.Application.CustomerVoices.GetCustomerVoices;

public sealed record GetCustomerVoicesQuery(
    Guid UserId,
    string? TokenType,
    string? OrganizationId)
    : IRequest<IReadOnlyList<CustomerVoice>>;

public sealed class GetCustomerVoicesQueryHandler(
    ICustomerVoiceRepository customerVoiceRepository)
    : IRequestHandler<
        GetCustomerVoicesQuery,
        IReadOnlyList<CustomerVoice>>
{
    public async Task<IReadOnlyList<CustomerVoice>> Handle(
        GetCustomerVoicesQuery request,
        CancellationToken ct)
    {
        if (request.TokenType == "customer" &&
            Guid.TryParse(request.OrganizationId, out var customerId))
        {
            return await customerVoiceRepository.GetByCustomerIdAsync(
                customerId,
                ct);
        }

        if (request.TokenType == "internal")
        {
            return await customerVoiceRepository.GetAllAsync(ct);
        }

        return [];
    }
}