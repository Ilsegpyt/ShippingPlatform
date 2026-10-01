
using BuildingBlocks.Application;
using Content.Application.Abstractions;
using Content.Application.Content;
using MediatR;

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
        // Step 1: Find the content item to delete.
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

        // Step 3: Extract local image URLs from the featured image and HTML body.
        var imageUrls = ContentImageHelper.ExtractLocalImageUrls(
            content.FeaturedImage,
            content.Body);

        // Step 4: Identify images that are not used by other content items.
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

        // Step 5: Delete the content record and save database changes first.
        contentRepository.Delete(content);

        await unitOfWork.SaveChangesAsync(ct);

        // Step 6: Delete image files only after the database save succeeds.
        foreach (var imageUrl in imagesToDelete)
        {
            await fileStorage.DeleteAsync(imageUrl, ct);
        }

        return Result.Success();
    }
}
