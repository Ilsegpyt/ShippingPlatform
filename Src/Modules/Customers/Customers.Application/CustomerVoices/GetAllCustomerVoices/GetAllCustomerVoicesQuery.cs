using Customers.Application.Abstractions;
using Customers.Domain.Entities;
using MediatR;

namespace Customers.Application.CustomerVoices.GetAllCustomerVoices;

public sealed record GetAllCustomerVoicesQuery
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
        return await customerVoiceRepository.GetAllAsync(ct);
    }
}