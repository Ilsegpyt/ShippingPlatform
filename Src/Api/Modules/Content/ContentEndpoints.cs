using Api.Modules.Content.CreateContent;
using Api.Modules.Content.GetContentById;
using Api.Modules.Content.GetContentChildren;
using Api.Modules.Content.GetRootContent;

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

    }
}