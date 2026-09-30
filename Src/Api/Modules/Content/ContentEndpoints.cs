using Api.Modules.Content.CreateContent;
using Api.Modules.Content.DeleteContent;
using Api.Modules.Content.GetContentById;
using Api.Modules.Content.GetContentChildren;
using Api.Modules.Content.GetRootContent;
using Api.Modules.Content.UpdateContent;
using Api.Modules.Content.UploadContentImage;

namespace Api.Modules.Content;

public static class ContentEndpoints
{
    public static void MapContentEndpoints(
        this IEndpointRouteBuilder app)
    {
        CreateContentEndpoint.Map(app);
        GetContentByIdEndpoint.Map(app);
        GetContentChildrenEndpoint.Map(app);
        GetRootContentEndpoint.Map(app);
        UpdateContentEndpoint.Map(app);
        DeleteContentEndpoint.Map(app);
        UploadContentImageEndpoint.Map(app);

    }
}