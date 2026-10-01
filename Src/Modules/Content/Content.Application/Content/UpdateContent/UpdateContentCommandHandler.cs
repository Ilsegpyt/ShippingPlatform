using BuildingBlocks.Application;
using Content.Application.Abstractions;
using Content.Application.Content;
using Content.Domain.Entities;
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

        // Step 2: Validate the requested parent.
        if (command.ParentId.HasValue)
        {
            var parent = await contentRepository.GetByIdAsync(
                command.ParentId.Value,
                ct);

            if (parent is null)
            {
                return Result.Failure(
                    "Parent content was not found.");
            }

            if (parent.Type is not ContentType.Category
                and not ContentType.Page)
            {
                return Result.Failure(
                    "Parent content must be a Category or Page.");
            }

            // Prevent the content from becoming its own parent.
            if (parent.Id == content.Id)
            {
                return Result.Failure(
                    "Content cannot be its own parent.");
            }

            // Prevent circular hierarchy by walking up the parent chain.
            var ancestorId = parent.ParentId;

            while (ancestorId.HasValue)
            {
                if (ancestorId.Value == content.Id)
                {
                    return Result.Failure(
                        "Circular content hierarchy is not allowed.");
                }

                var ancestor = await contentRepository.GetByIdAsync(
                    ancestorId.Value,
                    ct);

                if (ancestor is null)
                    break;

                ancestorId = ancestor.ParentId;
            }
        }

        // Step 3: Save the old image URLs before updating the content.
        var oldImageUrls = ContentImageHelper.ExtractLocalImageUrls(
            content.FeaturedImage,
            content.Body);

        // Step 4: Apply the requested changes.
        content.Update(
            command.ParentId,
            command.Title,
            command.Type,
            command.Body,
            command.FeaturedImage,
            command.LinkUrl);

        // Step 5: Save database changes before cleaning up old files.
        await unitOfWork.SaveChangesAsync(ct);

        // Step 6: Collect image URLs referenced after the update.
        var newImageUrls = ContentImageHelper.ExtractLocalImageUrls(
            content.FeaturedImage,
            content.Body);

        // Step 7: Find old images that are no longer used by this content.
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