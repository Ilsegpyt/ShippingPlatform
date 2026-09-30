using Content.Application.Abstractions;
using Microsoft.AspNetCore.Hosting;

namespace Content.Infrastructure.Storage;

public sealed class LocalContentFileStorage : IContentFileStorage
{
    private readonly IWebHostEnvironment _environment;

    public LocalContentFileStorage(IWebHostEnvironment environment)
    {
        _environment = environment;
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

        var storedFileName =
            $"{Guid.NewGuid():N}{extension}";

        var filePath = Path.Combine(
            uploadsFolder,
            storedFileName);

        await using var output =
            new FileStream(
                filePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None);

        await file.CopyToAsync(output, ct);

        return $"/uploads/content/{storedFileName}";
    }

    public Task DeleteAsync(
        string storageKey,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
            return Task.CompletedTask;

        var relativePath = storageKey
            .TrimStart('/')
            .Replace(
                '/',
                Path.DirectorySeparatorChar);

        var filePath = Path.Combine(
            _environment.ContentRootPath,
            "wwwroot",
            relativePath);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}