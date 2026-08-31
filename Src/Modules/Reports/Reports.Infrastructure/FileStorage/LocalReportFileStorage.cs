using Reports.Application.Abstractions;

namespace Reports.Infrastructure.FileStorage;

public sealed class LocalReportFileStorage : IReportFileStorage
{
    private readonly string _rootPath;

    public LocalReportFileStorage(string rootPath)
    {
        _rootPath = rootPath;

        Directory.CreateDirectory(_rootPath);
    }

    public async Task<string> SaveAsync(
        Stream file,
        string fileName,
        CancellationToken ct)
    {
        var storageKey = $"{Guid.NewGuid()}_{fileName}";

        var filePath = Path.Combine(_rootPath, storageKey);

        await using var output = new FileStream(
            filePath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None);

        await file.CopyToAsync(output, ct);

        return storageKey;
    }

    public Task DeleteAsync(
        string storageKey,
        CancellationToken ct)
    {
        var filePath = Path.Combine(_rootPath, storageKey);

        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }
    public Task<Stream> OpenReadAsync(
    string storageKey,
    CancellationToken ct)
    {
        var fullPath = Path.Combine(_rootPath, storageKey);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException(
                "Report file was not found.",
                fullPath);

        Stream stream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            4096,
            useAsync: true);

        return Task.FromResult(stream);
    }
}