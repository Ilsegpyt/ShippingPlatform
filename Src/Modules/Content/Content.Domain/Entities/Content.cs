namespace Content.Domain.Entities;

public sealed class Content
{
    public Guid Id { get; private set; }

    public Guid? ParentId { get; private set; }

    public string Title { get; private set; } = null!;

    public ContentType Type { get; private set; }

    public string? Body { get; private set; }

    public string? FeaturedImage { get; private set; }

    public string? LinkUrl { get; private set; }

    public Content? Parent { get; private set; }

    public ICollection<Content> Children { get; private set; } = new List<Content>();

    public static Content Create(
        Guid? parentId,
        string title,
        ContentType type,
        string? body,
        string? featuredImage,
        string? linkUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        return new Content(
            Guid.NewGuid(),
            parentId,
            title,
            type,
            body,
            featuredImage,
            linkUrl);
    }

    private Content()
    {
    }

    private Content(
        Guid id,
        Guid? parentId,
        string title,
        ContentType type,
        string? body,
        string? featuredImage,
        string? linkUrl)
    {
        Id = id;
        ParentId = parentId;
        Title = title;
        Type = type;
        Body = body;
        FeaturedImage = featuredImage;
        LinkUrl = linkUrl;
    }
}

public enum ContentType
{
    Category = 1,
    Page = 2,
    Post = 3,
    Link = 4
}