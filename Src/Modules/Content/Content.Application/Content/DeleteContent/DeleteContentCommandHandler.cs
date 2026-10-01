using BuildingBlocks.Application;
using Content.Application.Abstractions;
using MediatR;
using System.Text.RegularExpressions;

namespace Content.Application.Content.DeleteContent;

public sealed class DeleteContentCommandHandler(
    IContentRepository contentRepository,
    IContentUnitOfWork unitOfWork,
    IContentFileStorage fileStorage)
    : IRequestHandler<DeleteContentCommand, Result>
{
    public async Task<Result> Handle(
        DeleteContentCommand command,
        CancellationToken ct)
    {
        // Step 1: Find the content item that should be deleted.
        var content = await contentRepository.GetByIdAsync(
            command.Id,
            ct);

        if (content is null)
        {
            return Result.Failure("Content not found.");
        }

        // Step 2: Prevent deleting content that still has children.
        var children = await contentRepository.GetChildrenAsync(
            command.Id,
            ct);

        if (children.Count > 0)
        {
            return Result.Failure(
                "Cannot delete content that has children.");
        }

        // Step 3: Collect image URLs referenced by this content item.
        var imageUrls = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        AddLocalImageUrl(imageUrls, content.FeaturedImage);

        // Extract image sources from the HTML body.
        if (!string.IsNullOrWhiteSpace(content.Body))
        {
            var matches = Regex.Matches(
                content.Body,
                """src\s*=\s*["']([^"']+)["']""",
                RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                AddLocalImageUrl(imageUrls, match.Groups[1].Value);
            }
        }

        // Step 4: Keep only images that are not referenced by other content items.
        var imagesToDelete = new List<string>();

        foreach (var imageUrl in imageUrls)
        {
            var isUsedElsewhere =
                await contentRepository.IsImageUsedByOtherContentAsync(
                    imageUrl,
                    content.Id,
                    ct);

            if (!isUsedElsewhere)
            {
                imagesToDelete.Add(imageUrl);
            }
        }

        // Step 5: Delete the content record and save the database changes first.
        // This avoids deleting image files if the database operation fails.
        contentRepository.Delete(content);

        await unitOfWork.SaveChangesAsync(ct);

        // Step 6: Delete image files only after the database save succeeds.
        foreach (var imageUrl in imagesToDelete)
        {
            await fileStorage.DeleteAsync(imageUrl, ct);
        }

        return Result.Success();
    }

    // Adds only images stored in our content uploads directory.
    // External URLs and unrelated local paths are ignored.
    private static void AddLocalImageUrl(
        HashSet<string> imageUrls,
        string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return;
        }

        var value = imageUrl.Trim();

        // Convert absolute URLs into paths so they match stored relative URLs.
        if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            value = uri.AbsolutePath;
        }

        // Remove query strings and normalize URL separators.
        var queryIndex = value.IndexOf('?');

        if (queryIndex >= 0)
        {
            value = value[..queryIndex];
        }

        value = Uri.UnescapeDataString(value)
            .Replace('\\', '/');

        const string allowedPrefix = "/uploads/content/";

        // Only process files inside the content uploads directory.
        if (!value.StartsWith(
                allowedPrefix,
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        // Reject nested paths and invalid file names.
        var fileName = value[allowedPrefix.Length..];

        if (string.IsNullOrWhiteSpace(fileName)
            || fileName.Contains('/')
            || fileName is "." or "..")
        {
            return;
        }

        imageUrls.Add(allowedPrefix + fileName);
    }
}