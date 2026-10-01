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
            if (command.ParentId.Value == content.Id)
            {
                return Result.Failure(
                    "Content cannot be its own parent.");
            }

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

            // Prevent circular hierarchy and detect existing cycles.
            var visitedIds = new HashSet<Guid>();
            var ancestor = parent;

            while (true)
            {
                if (ancestor.Id == content.Id)
                {
                    return Result.Failure(
                        "Circular content hierarchy is not allowed.");
                }

                if (!visitedIds.Add(ancestor.Id))
                {
                    return Result.Failure(
                        "Invalid parent hierarchy detected.");
                }

                if (!ancestor.ParentId.HasValue)
                    break;

                var ancestorId = ancestor.ParentId.Value;

                var nextAncestor = await contentRepository.GetByIdAsync(
                    ancestorId,
                    ct);

                if (nextAncestor is null)
                {
                    return Result.Failure(
                        "Parent hierarchy is invalid.");
                }

                ancestor = nextAncestor;
            }
        }

        // Step 3: Prevent changing a parent with children to Post or Link.
        if (command.Type is not ContentType.Category
            and not ContentType.Page)
        {
            var children = await contentRepository.GetChildrenAsync(
                content.Id,
                ct);

            if (children.Count > 0)
            {
                return Result.Failure(
                    "Content with children must remain a Category or Page.");
            }
        }

        // Step 4: Save the old image URLs before updating the content.
        var oldImageUrls = ContentImageHelper.ExtractLocalImageUrls(
            content.FeaturedImage,
            content.Body);

        // Step 5: Apply the requested changes.
        content.Update(
            command.ParentId,
            command.Title,
            command.Type,
            command.Body,
            command.FeaturedImage,
            command.LinkUrl);

        // Step 6: Save database changes before cleaning up old files.
        await unitOfWork.SaveChangesAsync(ct);

        // Step 7: Collect image URLs referenced after the update.
        var newImageUrls = ContentImageHelper.ExtractLocalImageUrls(
            content.FeaturedImage,
            content.Body);

        // Step 8: Find old images that are no longer used by this content.
        var imagesToCheck = oldImageUrls.Except(
            newImageUrls,
            StringComparer.OrdinalIgnoreCase);

        foreach (var imageUrl in imagesToCheck)
        {
            try
            {
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