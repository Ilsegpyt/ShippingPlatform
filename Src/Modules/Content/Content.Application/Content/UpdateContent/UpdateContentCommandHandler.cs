using BuildingBlocks.Application;
using Content.Application.Abstractions;
using MediatR;

namespace Content.Application.Content.UpdateContent;

public sealed class UpdateContentCommandHandler(
    IContentRepository contentRepository,
    IContentUnitOfWork unitOfWork,
    IContentFileStorage fileStorage)
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
        var oldFeaturedImage = content.FeaturedImage;
        var oldBody = content.Body;

        // Step 3: Apply the requested changes.
        content.Update(
            command.ParentId,
            command.Title,
            command.Type,
            command.Body,
            command.FeaturedImage,
            command.LinkUrl);

        // Step 4: Save the database changes first.
        await unitOfWork.SaveChangesAsync(ct);

        // Step 5: Collect image URLs referenced before the update.
        var oldImageUrls = ContentImageHelper.ExtractLocalImageUrls(
            oldFeaturedImage,
            oldBody);

        // Step 6: Collect image URLs referenced after the update.
        var newImageUrls = ContentImageHelper.ExtractLocalImageUrls(
            content.FeaturedImage,
            content.Body);

        // Step 7: Check old images that are no longer used by this content.
        foreach (var oldImageUrl in oldImageUrls.Except(
                     newImageUrls,
                     StringComparer.OrdinalIgnoreCase))
        {
            // Delete an old image only when no other content item uses it.
            var isUsedElsewhere =
                await contentRepository.IsImageUsedByOtherContentAsync(
                    oldImageUrl,
                    content.Id,
                    ct);

            if (!isUsedElsewhere)
            {
                await fileStorage.DeleteAsync(oldImageUrl, ct);
            }
        }

        return Result.Success();
    }
}