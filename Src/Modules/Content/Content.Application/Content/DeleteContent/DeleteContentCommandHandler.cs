
using BuildingBlocks.Application;
using Content.Application.Abstractions;
using Content.Application.Content;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Content.Application.Content.DeleteContent;

public sealed class DeleteContentCommandHandler(
    IContentRepository contentRepository,
    IContentUnitOfWork unitOfWork,
    IContentFileStorage fileStorage,
    ILogger<DeleteContentCommandHandler> logger)
    : IRequestHandler<DeleteContentCommand, Result>
{
    public async Task<Result> Handle(
        DeleteContentCommand command,
        CancellationToken ct)
    {
        // Step 1: Find the content item to delete.
        var content = await contentRepository.GetByIdAsync(
            command.Id,
            ct);

        if (content is null)
            return Result.Failure("Content not found.");

        // Step 2: Prevent deleting content that still has children.
        var children = await contentRepository.GetChildrenAsync(
            command.Id,
            ct);

        if (children.Count > 0)
        {
            return Result.Failure(
                "Cannot delete content that has children.");
        }

        // Step 3: Extract local image URLs from the featured image and HTML body.
        var imageUrls = ContentImageHelper.ExtractLocalImageUrls(
            content.FeaturedImage,
            content.Body);

        // Step 4: Delete the content record and save database changes first.
        contentRepository.Delete(content);
        await unitOfWork.SaveChangesAsync(ct);

        // Step 5: Check and delete unreferenced images after the database save.
        foreach (var imageUrl in imageUrls)
        {
            try
            {
                // Check whether another content item still uses this image.
                var isUsedElsewhere =
                    await contentRepository.IsImageUsedByOtherContentAsync(
                        imageUrl,
                        content.Id,
                        ct);

                if (isUsedElsewhere)
                    continue;

                // Delete the file only when no other content item uses it.
                await fileStorage.DeleteAsync(imageUrl, ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // Log cleanup failures without undoing the deleted content record.
                logger.LogError(
                    ex,
                    "Failed to clean up content image {ImageUrl} " +
                    "after deleting content {ContentId}.",
                    imageUrl,
                    content.Id);
            }
        }

        return Result.Success();
    }
}
