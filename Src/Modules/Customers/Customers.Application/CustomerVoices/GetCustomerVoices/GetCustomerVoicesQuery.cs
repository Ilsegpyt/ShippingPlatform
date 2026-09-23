using Customers.Application.Abstractions;
using Customers.Domain.Entities;
using MediatR;

namespace Customers.Application.CustomerVoices.GetCustomerVoices;

public sealed record GetCustomerVoicesQuery(
    Guid CustomerId)
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
        return await customerVoiceRepository.GetByCustomerIdAsync(
            request.CustomerId,
            ct);
    }
}