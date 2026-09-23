using Customers.Application.Abstractions;
using Customers.Domain.Entities;
using MediatR;

namespace Customers.Application.CustomerVoices.GetAllCustomerVoices;

public sealed record GetAllCustomerVoicesQuery(
    Guid UserId,
    string? TokenType,
    string? OrganizationId)
    : IRequest<IReadOnlyList<CustomerVoice>>;

public sealed class GetAllCustomerVoicesQueryHandler(
    ICustomerVoiceRepository customerVoiceRepository)
    : IRequestHandler<
        GetAllCustomerVoicesQuery,
        IReadOnlyList<CustomerVoice>>
{
    public async Task<IReadOnlyList<CustomerVoice>> Handle(
        GetAllCustomerVoicesQuery request,
        CancellationToken ct)
    {
        if (request.TokenType == "customer" &&
            Guid.TryParse(request.OrganizationId, out var customerId))
        {
            return await customerVoiceRepository.GetByCustomerIdAsync(
                customerId,
                ct);
        }

        return await customerVoiceRepository.GetAllAsync(ct);
    }
}