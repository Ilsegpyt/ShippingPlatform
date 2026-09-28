using BuildingBlocks.Application;
using Content.Application.Abstractions;
using MediatR;

namespace Content.Application.Content.UpdateContent;

public sealed class UpdateContentCommandHandler(
    IContentRepository contentRepository,
    IContentUnitOfWork unitOfWork)
    : IRequestHandler<UpdateContentCommand, Result>
{
    public async Task<Result> Handle(
        UpdateContentCommand command,
        CancellationToken ct)
    {
        var content = await contentRepository.GetByIdAsync(
            command.Id,
            ct);

        if (content is null)
            return Result.Failure("Content not found.");

        content.Update(
            command.ParentId,
            command.Title,
            command.Type,
            command.Body,
            command.FeaturedImage,
            command.LinkUrl);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}