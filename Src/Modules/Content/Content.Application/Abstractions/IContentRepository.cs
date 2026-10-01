using ContentEntity = Content.Domain.Entities.Content;

namespace Content.Application.Abstractions;

public interface IContentRepository
{
    void Add(ContentEntity content);

    Task<ContentEntity?> GetByIdAsync(
        Guid id,
        CancellationToken ct);

    Task<IReadOnlyList<ContentEntity>> ListAsync(
        CancellationToken ct);

    Task<IReadOnlyList<ContentEntity>> GetChildrenAsync(
        Guid? parentId,
        CancellationToken ct);

    void Delete(ContentEntity content);

    // Checks whether an image URL is referenced by another content item.
    // Used to prevent deleting an image that is still in use.
    Task<bool> IsImageUsedByOtherContentAsync(
        string imageUrl,
        Guid excludedContentId,
        CancellationToken ct);
}