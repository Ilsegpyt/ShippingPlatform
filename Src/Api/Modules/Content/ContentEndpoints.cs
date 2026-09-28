using Api.Modules.Content.CreateContent;

namespace Api.Modules.Content;

public static class ContentEndpoints
{
    public static void MapContentEndpoints(
        this IEndpointRouteBuilder app)
    {
        CreateContentEndpoint.Map(app);
    }
}