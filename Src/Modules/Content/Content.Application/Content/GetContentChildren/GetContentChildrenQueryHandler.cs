using BuildingBlocks.Application;
using Content.Application.Abstractions;
using Content.Application.Content.GetContentById;
using MediatR;

namespace Content.Application.Content.GetContentChildren;

public sealed class GetContentChildrenQueryHandler(
    IContentRepository contentRepository)
    : IRequestHandler<
        GetContentChildrenQuery,
        Result<IReadOnlyList<ContentDto>>>
{
    public async Task<Result<IReadOnlyList<ContentDto>>> Handle(
        GetContentChildrenQuery query,
        CancellationToken ct)
    {
        var contents = await contentRepository.GetChildrenAsync(
            query.ParentId,
            ct);

        var result = contents
            .Select(content => new ContentDto(
                content.Id,
                content.ParentId,
                content.Title,
                content.Type,
                content.Body,
                content.FeaturedImage,
                content.LinkUrl))
            .ToList();

        return Result.Success<IReadOnlyList<ContentDto>>(result);
    }
}