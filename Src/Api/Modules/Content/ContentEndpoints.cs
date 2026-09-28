using Api.Modules.Content.CreateContent;
using Api.Modules.Content.GetContentById;

namespace Api.Modules.Content;

public static class ContentEndpoints
{
    public static void MapContentEndpoints(
        this IEndpointRouteBuilder app)
    {
        CreateContentEndpoint.Map(app);
        GetContentByIdEndpoint.Map(app);
    }
}