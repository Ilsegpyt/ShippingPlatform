using Operations.Domain.Entities;

namespace Operations.Application.Abstractions;

public interface ITransporterRepository
{
    Task<IReadOnlyList<Transporter>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<Transporter?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> TransporterNameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Transporter transporter, CancellationToken cancellationToken = default);
}

