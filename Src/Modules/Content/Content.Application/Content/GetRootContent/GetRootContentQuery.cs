using BuildingBlocks.Application;
using Content.Application.Content.GetContentById;
using MediatR;

namespace Content.Application.Content.GetRootContent;

public sealed record GetRootContentQuery
    : IRequest<Result<IReadOnlyList<ContentDto>>>;