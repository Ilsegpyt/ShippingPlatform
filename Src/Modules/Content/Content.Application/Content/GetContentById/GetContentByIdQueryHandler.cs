using BuildingBlocks.Application;
using Content.Application.Abstractions;
using MediatR;

namespace Content.Application.Content.GetContentById;

public sealed class GetContentByIdQueryHandler(
    IContentRepository contentRepository)
    : IRequestHandler<GetContentByIdQuery, Result<ContentDto>>
{
    public async Task<Result<ContentDto>> Handle(
        GetContentByIdQuery query,
        CancellationToken ct)
    {
        var content = await contentRepository.GetByIdAsync(
            query.Id,
            ct);

        if (content is null)
            return Result.Failure<ContentDto>(
                "Content not found.");

        var dto = new ContentDto(
            content.Id,
            content.ParentId,
            content.Title,
            content.Type,
            content.Body,
            content.FeaturedImage,
            content.LinkUrl);

        return Result.Success(dto);
    }
}