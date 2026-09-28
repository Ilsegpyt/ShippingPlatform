using BuildingBlocks.Application;
using Content.Domain.Entities;
using MediatR;

namespace Content.Application.Content.UpdateContent;

public sealed record UpdateContentCommand(
    Guid Id,
    Guid? ParentId,
    string Title,
    ContentType Type,
    string? Body,
    string? FeaturedImage,
    string? LinkUrl
) : IRequest<Result>;