using BuildingBlocks.Application;
using Content.Application.Abstractions;
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
        var content = ContentEntity.Create(
            command.ParentId,
            command.Title,
            command.Type,
            command.Body,
            command.FeaturedImage,
            command.LinkUrl);

        contentRepository.Add(content);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(content.Id);
    }
}