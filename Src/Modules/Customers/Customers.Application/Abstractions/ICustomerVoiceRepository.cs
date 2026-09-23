using Customers.Domain.Entities;

namespace Customers.Application.Abstractions;

public interface ICustomerVoiceRepository
{
    Task AddAsync(
        CustomerVoice customerVoice,
        CancellationToken ct = default);

    Task<IReadOnlyList<CustomerVoice>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken ct = default);

    Task<IReadOnlyList<CustomerVoice>> GetAllAsync(
        CancellationToken ct = default);

    Task<CustomerVoice?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);
}