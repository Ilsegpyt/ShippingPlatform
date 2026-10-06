using BuildingBlocks.Application;
using Content.Application.Abstractions;
using Content.Domain.Entities;
using ContentEntity = Content.Domain.Entities.Content;
using MediatR;

namespace Content.Application.Content.CreateContent;

public sealed class CreateContentCommandHandler(
    IContentRepository contentRepository,
    IContentUnitOfWork unitOfWork)
    : IRequestHandler<CreateContentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateContentCommand command,
        CancellationToken ct)
    {
        if (command.ParentId.HasValue)
        {
            var parent = await contentRepository.GetByIdAsync(
                command.ParentId.Value,
                ct);

            if (parent is null)
            {
                return Result.Failure<Guid>(
                    "Parent content was not found.");
            }

            if (parent.Type is not ContentType.Category
                and not ContentType.Page)
            {
                return Result.Failure<Guid>(
                    "Parent content must be a Category or Page.");
            }
        }

        var content = ContentEntity.Create(
            command.ParentId,
            command.Title,
            command.Type,
            command.Body,
            command.FeaturedImage,
            command.LinkUrl,
            command.Category,
            command.PublishedAt);

        contentRepository.Add(content);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(content.Id);
    }
}