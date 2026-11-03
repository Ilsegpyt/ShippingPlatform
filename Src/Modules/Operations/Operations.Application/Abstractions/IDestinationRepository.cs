using Operations.Domain.Entities;

namespace Operations.Application.Abstractions;

public interface IDestinationRepository
{
    Task<IReadOnlyList<Destination>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<Destination?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DestinationNameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Destination destination, CancellationToken cancellationToken = default);
}

