namespace Content.Application.Abstractions;

public interface IContentFileStorage
{
    Task<string> SaveAsync(
        Stream file,
        string fileName,
        CancellationToken ct);

    Task DeleteAsync(
        string storageKey,
        CancellationToken ct);
}