namespace Shipments.Application.Abstractions;

public interface IFileStorage
{
    Task<string> SaveAsync(
        Stream file,
        string fileName,
        CancellationToken ct);

    Task DeleteAsync(
        string storageKey,
        CancellationToken ct);

    Task<Stream> OpenReadAsync(
        string storageKey,
        CancellationToken ct);
}