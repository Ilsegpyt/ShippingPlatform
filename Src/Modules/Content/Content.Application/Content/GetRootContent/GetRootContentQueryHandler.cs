using BuildingBlocks.Application;
using Content.Application.Abstractions;
using Content.Application.Content.GetContentById;
using MediatR;

namespace Content.Application.Content.GetRootContent;

public sealed class GetRootContentQueryHandler(
    IContentRepository contentRepository)
    : IRequestHandler<
        GetRootContentQuery,
        Result<IReadOnlyList<ContentDto>>>
{
    public async Task<Result<IReadOnlyList<ContentDto>>> Handle(
        GetRootContentQuery query,
        CancellationToken ct)
    {
        var contents = await contentRepository.GetChildrenAsync(
            null,
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