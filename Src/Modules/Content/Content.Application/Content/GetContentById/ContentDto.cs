using Content.Domain.Entities;

namespace Content.Application.Content.GetContentById;

public sealed record ContentDto(
    Guid Id,
    Guid? ParentId,
    string Title,
    ContentType Type,
    string? Body,
    string? FeaturedImage,
    string? LinkUrl
);