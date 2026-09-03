using Shipments.Domain.Declarations;

namespace Shipments.Application.Abstractions;

public interface IDeclarationFileRepository
{
    Task AddAsync(
        DeclarationFile declarationFile,
        CancellationToken ct = default);

    Task<IReadOnlyList<DeclarationFile>> GetByIdsAsync(
    Guid shipmentId,
    IReadOnlyCollection<Guid> declarationFileIds,
    CancellationToken ct = default);
    void RemoveRange(
    IReadOnlyCollection<DeclarationFile> declarationFiles);

    Task<IReadOnlyList<DeclarationFile>> GetByShipmentIdsAsync(
    IReadOnlyCollection<Guid> shipmentIds,
    CancellationToken ct = default);

}