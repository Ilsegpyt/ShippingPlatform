
using BuildingBlocks.Application;
using Content.Application.Abstractions;
using Content.Application.Content;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Content.Application.Content.UpdateContent;

public sealed class UpdateContentCommandHandler(
    IContentRepository contentRepository,
    IContentUnitOfWork unitOfWork,
    IContentFileStorage fileStorage,
    ILogger<UpdateContentCommandHandler> logger)
    : IRequestHandler<UpdateContentCommand, Result>
{
    public async Task<Result> Handle(
        UpdateContentCommand command,
        CancellationToken ct)
    {
        // Step 1: Find the content item to update.
        var content = await contentRepository.GetByIdAsync(
            command.Id,
            ct);

        if (content is null)
            return Result.Failure("Content not found.");

        // Step 2: Save the old image URLs before updating the content.
        var oldImageUrls = ContentImageHelper.ExtractLocalImageUrls(
            content.FeaturedImage,
            content.Body);

        // Step 3: Apply the requested changes.
        content.Update(
            command.ParentId,
            command.Title,
            command.Type,
            command.Body,
            command.FeaturedImage,
            command.LinkUrl);

        // Step 4: Save database changes before cleaning up old files.
        await unitOfWork.SaveChangesAsync(ct);

        // Step 5: Collect image URLs referenced after the update.
        var newImageUrls = ContentImageHelper.ExtractLocalImageUrls(
            content.FeaturedImage,
            content.Body);

        // Step 6: Find old images that are no longer used by this content.
        var imagesToCheck = oldImageUrls.Except(
            newImageUrls,
            StringComparer.OrdinalIgnoreCase);

        foreach (var imageUrl in imagesToCheck)
        {
            try
            {
                // Delete an image only when no other content item uses it.
                var isUsedElsewhere =
                    await contentRepository.IsImageUsedByOtherContentAsync(
                        imageUrl,
                        content.Id,
                        ct);

                if (isUsedElsewhere)
                    continue;

                await fileStorage.DeleteAsync(imageUrl, ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // Log cleanup failures without undoing the saved content update.
                logger.LogError(
                    ex,
                    "Failed to clean up old content image {ImageUrl} " +
                    "after updating content {ContentId}.",
                    imageUrl,
                    content.Id);
            }
        }

        return Result.Success();
    }
}
