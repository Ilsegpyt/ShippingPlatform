using Content.Domain.Entities;
using BuildingBlocks.Application;
using MediatR;

namespace Content.Application.Content.CreateContent;

public sealed record CreateContentCommand(
    Guid? ParentId,
    string Title,
    ContentType Type,
    string? Body,
    string? FeaturedImage,
    string? LinkUrl
) : IRequest<Result<Guid>>;