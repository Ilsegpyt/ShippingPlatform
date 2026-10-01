
using Content.Application.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Content.Infrastructure.Storage;

public sealed class LocalContentFileStorage : IContentFileStorage
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<LocalContentFileStorage> _logger;

    public LocalContentFileStorage(
        IWebHostEnvironment environment,
        ILogger<LocalContentFileStorage> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async Task<string> SaveAsync(
        Stream file,
        string fileName,
        CancellationToken ct)
    {
        var uploadsFolder = Path.Combine(
            _environment.ContentRootPath,
            "wwwroot",
            "uploads",
            "content");

        Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(fileName);
        var storedFileName = $"{Guid.NewGuid():N}{extension}";

        var filePath = Path.Combine(
            uploadsFolder,
            storedFileName);

        try
        {
            await using var output = new FileStream(
                filePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None);

            await file.CopyToAsync(output, ct);

            _logger.LogInformation(
                "Content image uploaded successfully. File: {FileName}",
                storedFileName);

            return $"/uploads/content/{storedFileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to upload content image. File: {FileName}",
                storedFileName);

            throw;
        }
    }

    public Task DeleteAsync(
        string storageKey,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
            return Task.CompletedTask;

        // Accept only file names inside the content uploads folder.
        var fileName = storageKey
            .Replace('\\', '/')
            .Split('/')
            .Last();

        if (string.IsNullOrWhiteSpace(fileName)
            || fileName is "." or ".."
            || fileName != storageKey.Replace('\\', '/').Split('/').Last())
        {
            _logger.LogWarning(
                "Invalid content image storage key: {StorageKey}",
                storageKey);

            return Task.CompletedTask;
        }

        var uploadsFolder = Path.GetFullPath(
            Path.Combine(
                _environment.ContentRootPath,
                "wwwroot",
                "uploads",
                "content"));

        var filePath = Path.GetFullPath(
            Path.Combine(uploadsFolder, fileName));

        // Ensure the resolved path stays inside the uploads folder.
        if (!filePath.StartsWith(
                uploadsFolder + Path.DirectorySeparatorChar,
                StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(
                "Rejected content image path outside uploads folder.");

            return Task.CompletedTask;
        }

        try
        {
            ct.ThrowIfCancellationRequested();

            if (!File.Exists(filePath))
            {
                _logger.LogDebug(
                    "Content image file was not found. File: {FileName}",
                    fileName);

                return Task.CompletedTask;
            }

            File.Delete(filePath);

            _logger.LogInformation(
                "Content image deleted successfully. File: {FileName}",
                fileName);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            // Log the failure without throwing it back to the caller.
            _logger.LogError(
                ex,
                "Failed to delete content image. File: {FileName}",
                fileName);
        }

        return Task.CompletedTask;
    }
}
