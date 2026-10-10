using BuildingBlocks.Application;
using Content.Application.Abstractions;
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
        var content = await contentRepository.GetByIdAsync(
            command.Id,
            ct);

        if (content is null)
            return Result.Failure("Content not found.");

        var children = await contentRepository.GetChildrenAsync(
            command.Id,
            ct);

        if (children.Count > 0)
        {
            return Result.Failure(
                "Cannot delete content that has children.");
        }

        var imageUrls = ContentImageHelper.ExtractLocalImageUrls(
            content.FeaturedImage,
            content.Body);

        contentRepository.Delete(content);
        await unitOfWork.SaveChangesAsync(ct);

        foreach (var imageUrl in imageUrls)
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
                    "Failed to clean up content image {ImageUrl} after deleting content {ContentId}.",
                    imageUrl,
                    content.Id);
            }
        }

        return Result.Success();
    }
}
