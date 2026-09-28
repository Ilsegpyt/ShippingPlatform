using BuildingBlocks.Application;
using Content.Application.Content.GetContentById;
using MediatR;

namespace Content.Application.Content.GetContentChildren;

public sealed record GetContentChildrenQuery(
    Guid ParentId
) : IRequest<Result<IReadOnlyList<ContentDto>>>;