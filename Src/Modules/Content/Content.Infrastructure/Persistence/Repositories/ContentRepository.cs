
using ContentEntity = Content.Domain.Entities.Content;
using Content.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Content.Infrastructure.Persistence.Repositories;

public sealed class ContentRepository : IContentRepository
{
    private readonly ContentDbContext _dbContext;

    public ContentRepository(ContentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(ContentEntity content)
    {
        _dbContext.Contents.Add(content);
    }

    public async Task<ContentEntity?> GetByIdAsync(
        Guid id,
        CancellationToken ct)
    {
        return await _dbContext.Contents
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<ContentEntity>> ListAsync(
        CancellationToken ct)
    {
        return await _dbContext.Contents
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ContentEntity>> GetChildrenAsync(
        Guid? parentId,
        CancellationToken ct)
    {
        return await _dbContext.Contents
            .AsNoTracking()
            .Where(x => x.ParentId == parentId)
            .ToListAsync(ct);
    }

    // Checks whether another content item uses the specified image.
    public async Task<bool> IsImageUsedByOtherContentAsync(
        string imageUrl,
        Guid excludedContentId,
        CancellationToken ct)
    {
        // Normalize the image URL before comparing it with other references.
        var targetImage = NormalizeContentImageUrl(imageUrl);

        // Treat unrecognized paths as unsafe to delete.
        if (targetImage is null)
        {
            return true;
        }

        // Load image references from all other content items.
        var otherContents = await _dbContext.Contents
            .AsNoTracking()
            .Where(x => x.Id != excludedContentId)
            .Select(x => new
            {
                x.FeaturedImage,
                x.Body
            })
            .ToListAsync(ct);

        foreach (var content in otherContents)
        {
            // Check whether another item uses this as its featured image.
            var featuredImage =
                NormalizeContentImageUrl(content.FeaturedImage);

            if (featuredImage is not null &&
                string.Equals(
                    featuredImage,
                    targetImage,
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Extract image URLs from the HTML body.
            if (string.IsNullOrWhiteSpace(content.Body))
            {
                continue;
            }

            var imageSources = Regex.Matches(
                content.Body,
                """src\s*=\s*["']([^"']+)["']""",
                RegexOptions.IgnoreCase);

            foreach (Match match in imageSources)
            {
                var imageSource = NormalizeContentImageUrl(
                    match.Groups[1].Value);

                // Stop if another content item references the same image.
                if (imageSource is not null &&
                    string.Equals(
                        imageSource,
                        targetImage,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        // No other content item references this image.
        return false;
    }

    // Converts absolute URLs to paths and accepts only content-upload files.
    private static string? NormalizeContentImageUrl(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return null;
        }

        var value = imageUrl.Trim();

        // Extract the path from absolute URLs, such as localhost image URLs.
        if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            value = uri.AbsolutePath;
        }

        // Remove query strings.
        var queryIndex = value.IndexOf('?');

        if (queryIndex >= 0)
        {
            value = value[..queryIndex];
        }

        value = Uri.UnescapeDataString(value)
            .Replace('\\', '/');

        const string allowedPrefix = "/uploads/content/";

        // Ignore external images and files outside the content uploads folder.
        if (!value.StartsWith(
                allowedPrefix,
                StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        // Accept only a file directly inside the uploads folder.
        var fileName = value[allowedPrefix.Length..];

        if (string.IsNullOrWhiteSpace(fileName) ||
            fileName.Contains('/') ||
            fileName is "." or "..")
        {
            return null;
        }

        return allowedPrefix + fileName;
    }

    public void Delete(ContentEntity content)
    {
        _dbContext.Contents.Remove(content);
    }
}
