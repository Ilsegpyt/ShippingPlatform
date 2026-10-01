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

    // Checks whether another content item still uses the specified image.
    public async Task<bool> IsImageUsedByOtherContentAsync(
        string imageUrl,
        Guid excludedContentId,
        CancellationToken ct)
    {
        // Normalize the URL so relative and absolute URLs can be compared.
        var targetImage = NormalizeContentImageUrl(imageUrl);

        // External images and URLs outside our uploads folder must not be treated as local files.
        if (targetImage is null)
        {
            return true;
        }

        // Load image references from all content items except the one being deleted.
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
            // Check whether another content item uses this image as its featured image.
            if (NormalizeContentImageUrl(content.FeaturedImage) is string featuredImage
                && string.Equals(
                    featuredImage,
                    targetImage,
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Extract image sources from the HTML body and check for references.
            if (!string.IsNullOrWhiteSpace(content.Body))
            {
                var imageSources = Regex.Matches(
                    content.Body,
                    """src\s*=\s*["']([^"']+)["']""",
                    RegexOptions.IgnoreCase);

                foreach (Match match in imageSources)
                {
                    var imageSource = NormalizeContentImageUrl(
                        match.Groups[1].Value);

                    if (imageSource is not null
                        && string.Equals(
                            imageSource,
                            targetImage,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
        }

        // No other content item references this image.
        return false;
    }

    // Removes the host and query string, then accepts only our content-upload paths.
    private static string? NormalizeContentImageUrl(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return null;
        }

        var value = imageUrl.Trim();

        // Convert absolute URLs, such as http://localhost:5250/uploads/content/a.png,
        // into their path so they match relative URLs stored in the database.
        if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            value = uri.AbsolutePath;
        }

        // Ignore query strings and normalize URL separators.
        var queryIndex = value.IndexOf('?');

        if (queryIndex >= 0)
        {
            value = value[..queryIndex];
        }

        value = Uri.UnescapeDataString(value)
            .Replace('\\', '/');

        // Only allow images inside the content uploads directory.
        const string allowedPrefix = "/uploads/content/";

        if (!value.StartsWith(
                allowedPrefix,
                StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        // Reject nested paths; uploaded files should be directly inside this folder.
        var fileName = value[allowedPrefix.Length..];

        if (string.IsNullOrWhiteSpace(fileName)
            || fileName.Contains('/')
            || fileName is "." or "..")
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